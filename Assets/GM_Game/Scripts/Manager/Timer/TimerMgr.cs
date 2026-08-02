using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Manager
{
    /**
     * Title:统一时间调度管理器
     * Desciption:
     * 提供 Delay / Interval / Repeat / Cancel / Pause / Resume / DelayAsync / ServerTime。
     * 由 TimerDriver 每帧驱动，业务侧禁止自行 Update 计时。
     **/
    public class TimerMgr : Singleton<TimerMgr>
    {
        private const int InitialCapacity = 64;
        private const int InfiniteRepeat = -1;

        // 活跃任务列表
        private readonly List<TimerTask> _activeTasks = new List<TimerTask>(InitialCapacity);
        // 任务池 用于归还任务
        private readonly Stack<TimerTask> _taskPool = new Stack<TimerTask>(InitialCapacity);
        // 任务映射表 用于快速查找任务
        private readonly Dictionary<int, TimerTask> _taskMap = new Dictionary<int, TimerTask>(InitialCapacity);
        // 待移除任务ID列表 用于批量移除任务
        private readonly List<int> _pendingRemoveIds = new List<int>(32);

        private int _nextId = 1;
        private bool _initialized;
        private bool _globalPaused;
        private TimerDriver _driver;

        /// <summary>
        /// 服务器时间相对本地时间的偏移（秒）。
        /// </summary>
        private double _serverTimeOffsetSeconds;

        public bool IsInitialized => _initialized;
        public bool IsGlobalPaused => _globalPaused;
        public int ActiveCount => _activeTasks.Count;

        /// <summary>
        /// 由 Global 在启动时注入父节点并创建驱动器。
        /// parent 为空时会自建 DontDestroyOnLoad 根节点（便于单独跑主城场景调试）。
        /// </summary>
        public void Initialize(Transform parent = null)
        {
            // 驱动器已被场景卸载销毁时，需要重建
            if (_initialized && _driver != null)
            {
                return;
            }

            Transform root = parent;
            if (root == null)
            {
                GameObject rootGo = new GameObject("TimerRoot");
                UnityEngine.Object.DontDestroyOnLoad(rootGo);
                root = rootGo.transform;
            }

            GameObject driverGo = new GameObject("TimerDriver");
            driverGo.transform.SetParent(root, false);
            _driver = driverGo.AddComponent<TimerDriver>();
            _initialized = true;
        }

        /// <summary>
        /// 确保已初始化；未初始化时自动创建驱动器。
        /// </summary>
        private void EnsureInitialized()
        {
            if (_initialized && _driver != null)
            {
                return;
            }

            Initialize(null);
        }

        /// <summary>
        /// 延迟一次执行。
        /// </summary>
        public TimerHandle Delay(
            float seconds,
            Action callback,
            TimerType type = TimerType.GameTime,
            UnityEngine.Object owner = null,
            string debugName = null)
        {
            return Schedule(seconds, 1, callback, type, owner, debugName);
        }

        /// <summary>
        /// 无限循环执行。
        /// </summary>
        public TimerHandle Interval(
            float interval,
            Action callback,
            TimerType type = TimerType.GameTime,
            UnityEngine.Object owner = null,
            string debugName = null)
        {
            return Schedule(interval, InfiniteRepeat, callback, type, owner, debugName);
        }

        /// <summary>
        /// 按次数循环执行；repeatCount &lt;= 0 视为无限。
        /// </summary>
        public TimerHandle Repeat(
            float interval,
            int repeatCount,
            Action callback,
            TimerType type = TimerType.GameTime,
            UnityEngine.Object owner = null,
            string debugName = null)
        {
            int count = repeatCount <= 0 ? InfiniteRepeat : repeatCount;
            return Schedule(interval, count, callback, type, owner, debugName);
        }

        /// <summary>
        /// 异步延迟（可取消）。
        /// </summary>
        public UniTask DelayAsync(
            float seconds,
            TimerType type = TimerType.GameTime,
            CancellationToken cancellationToken = default,
            UnityEngine.Object owner = null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled(cancellationToken);
            }

            UniTaskCompletionSource utcs = new UniTaskCompletionSource();
            TimerHandle handle = Schedule(
                seconds,
                1,
                null,
                type,
                owner,
                "DelayAsync",
                utcs);

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    handle.Cancel();
                    utcs.TrySetCanceled(cancellationToken);
                });
            }

            return utcs.Task;
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public void Cancel(TimerHandle handle)
        {
            if (!TryGetTask(handle, out TimerTask task))
            {
                return;
            }

            MarkCancelled(task);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public void Pause(TimerHandle handle)
        {
            if (!TryGetTask(handle, out TimerTask task))
            {
                return;
            }

            task.IsPaused = true;
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public void Resume(TimerHandle handle)
        {
            if (!TryGetTask(handle, out TimerTask task))
            {
                return;
            }

            task.IsPaused = false;
        }

        /// <summary>
        /// 暂停所有任务
        /// </summary>
        /// <returns></returns>
        public void PauseAll()
        {
            _globalPaused = true;
        }

        /// <summary>
        /// 恢复所有任务
        /// </summary>
        /// <returns></returns>
        public void ResumeAll()
        {
            _globalPaused = false;
        }

        /// <summary>
        /// 判断句柄是否有效
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool IsHandleValid(TimerHandle handle)
        {
            return TryGetTask(handle, out _);
        }

        /// <summary>
        /// 用服务器时间戳（秒）校准本地偏移。
        /// </summary>
        /// <param name="serverUnixSeconds"></param>
        public void SyncServerTime(long serverUnixSeconds)
        {
            double clientUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _serverTimeOffsetSeconds = serverUnixSeconds - clientUnix;
        }

        /// <summary>
        /// 用服务器时间戳（毫秒）校准本地偏移。
        /// </summary>
        /// <param name="serverUnixMilliseconds"></param>
        public void SyncServerTimeMs(long serverUnixMilliseconds)
        {
            double clientUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _serverTimeOffsetSeconds = (serverUnixMilliseconds - clientUnixMs) / 1000.0;
        }

        /// <summary>
        /// 获取服务器当前UTC时间
        /// </summary>
        /// <returns></returns>
        public DateTime ServerNowUtc()
        {
            return DateTime.UtcNow.AddSeconds(_serverTimeOffsetSeconds);
        }

        /// <summary>
        /// 获取服务器当前本地时间
        /// </summary>
        /// <returns></returns>
        public DateTime ServerNowLocal()
        {
            return ServerNowUtc().ToLocalTime();
        }

        /// <summary>
        /// 获取服务器当前Unix时间戳
        /// </summary>
        /// <returns></returns>
        public long ServerUnixSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long)_serverTimeOffsetSeconds;
        }

        /// <summary>
        /// 取消指定 Owner 绑定的全部任务（UI 关闭时调用）。
        /// </summary>
        /// <param name="owner"></param>
        public void CancelByOwner(UnityEngine.Object owner)
        {
            if (owner == null)
            {
                return;
            }

            for (int i = 0; i < _activeTasks.Count; i++)
            {
                TimerTask task = _activeTasks[i];
                if (task == null || task.IsCancelled)
                {
                    continue;
                }

                if (ReferenceEquals(task.Owner, owner))
                {
                    MarkCancelled(task);
                }
            }
        }

        /// <summary>
        /// 清空全部任务（切账号/回登录等场景）。
        /// </summary>
        /// <returns></returns>
        public void ClearAll()
        {
            _pendingRemoveIds.Clear();
            for (int i = 0; i < _activeTasks.Count; i++)
            {
                TimerTask task = _activeTasks[i];
                if (task == null)
                {
                    continue;
                }

                MarkCancelled(task);
                _pendingRemoveIds.Add(task.Id);
            }

            FlushRemoved();
        }

        /// <summary>
        /// 打印任务信息
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            StringBuilder sb = new StringBuilder(256);
            sb.AppendLine($"[TimerMgr] Active={_activeTasks.Count}, GlobalPaused={_globalPaused}");
            sb.AppendLine("ID\tLeft\tRepeat\tType\tPaused\tName");

            for (int i = 0; i < _activeTasks.Count; i++)
            {
                TimerTask task = _activeTasks[i];
                if (task == null || task.IsCancelled)
                {
                    continue;
                }

                float left = Mathf.Max(0f, task.Delay - task.Elapsed);
                string name = string.IsNullOrEmpty(task.DebugName)
                    ? (task.Callback?.Method.Name ?? "null")
                    : task.DebugName;
                string repeat = task.IsInfinite
                    ? "Inf"
                    : $"{task.ExecutedCount}/{task.RepeatCount}";

                sb.Append(task.Id).Append('\t')
                    .Append(left.ToString("F2")).Append('\t')
                    .Append(repeat).Append('\t')
                    .Append(task.Type).Append('\t')
                    .Append(task.IsPaused).Append('\t')
                    .AppendLine(name);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 由 TimerDriver 每帧调用。Update 中禁止额外 GC 分配。
        /// </summary>
        internal void Tick(float deltaTime, float unscaledDeltaTime)
        {
            if (!_initialized || _activeTasks.Count == 0)
            {
                return;
            }

            _pendingRemoveIds.Clear();  // 清空待移除任务ID列表

            for (int i = 0; i < _activeTasks.Count; i++)
            {
                TimerTask task = _activeTasks[i];
                if (task == null || task.IsCancelled)
                {
                    if (task != null)
                    {
                        _pendingRemoveIds.Add(task.Id);  // 将任务ID添加到待移除任务ID列表
                    }

                    continue;
                }

                if (task.IsOwnerDestroyed())
                {
                    MarkCancelled(task);  // 标记任务为取消
                    _pendingRemoveIds.Add(task.Id);
                    continue;
                }

                if (task.IsPaused)
                {
                    continue;
                }

                // 全局暂停只影响游戏时间任务，真实时间继续走
                if (_globalPaused && task.Type == TimerType.GameTime)
                {
                    continue;
                }

                float dt = task.Type == TimerType.GameTime ? deltaTime : unscaledDeltaTime;  // 获取时间间隔
                if (dt <= 0f)
                {
                    continue;
                }

                task.Elapsed += dt;  // 累加已执行时间
                if (task.Elapsed < task.Delay)  // 如果已执行时间小于延迟时间，则继续等待
                {
                    continue;
                }

                Fire(task);  // 执行任务

                if (task.IsCancelled)
                {
                    _pendingRemoveIds.Add(task.Id);  // 将任务ID添加到待移除任务ID列表
                    continue;
                }

                bool finished = !task.IsInfinite && task.ExecutedCount >= task.RepeatCount;  // 如果任务不是无限循环但已执行次数达到重复次数，则任务完成
                if (finished)
                {
                    MarkCancelled(task);  // 标记任务为取消
                    _pendingRemoveIds.Add(task.Id);
                }
                else
                {
                    // 防止卡顿导致一次 Tick 跳过多拍：只回扣一个周期
                    task.Elapsed -= task.Delay;  // 回扣一个周期
                    if (task.Elapsed >= task.Delay)  // 如果已执行时间大于等于延迟时间，则重置已执行时间
                    {
                        task.Elapsed = 0f;  // 重置已执行时间
                    }
                }
            }

            FlushRemoved();  // 刷新已移除的任务
        }

        /// <summary>
        /// 调度一个任务
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="repeatCount"></param>
        /// <param name="callback"></param>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="debugName"></param>
        /// <param name="completionSource"></param>
        /// <returns></returns>
        private TimerHandle Schedule(
            float delay,
            int repeatCount,
            Action callback,
            TimerType type,
            UnityEngine.Object owner,
            string debugName,
            UniTaskCompletionSource completionSource = null)
        {
            EnsureInitialized();

            if (delay < 0f)
            {
                delay = 0f;
            }

            if (callback == null && completionSource == null)
            {
                Debug.LogError("[TimerMgr] callback 与 DelayAsync 完成源不能同时为空。");
                return TimerHandle.Invalid;
            }

            TimerTask task = RentTask();
            task.Id = _nextId++;
            task.Version++;
            task.Delay = delay;
            task.Elapsed = 0f;
            task.RepeatCount = repeatCount;
            task.ExecutedCount = 0;
            task.Callback = callback;
            task.Type = type;
            task.IsPaused = false;
            task.IsCancelled = false;
            task.Owner = owner;
            task.CompletionSource = completionSource;
            task.DebugName = debugName;

            _activeTasks.Add(task);
            _taskMap[task.Id] = task;

            return new TimerHandle(task.Id, task.Version);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="task"></param>
        private void Fire(TimerTask task)
        {
            task.ExecutedCount++;

            try
            {
                task.Callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            // 如果任务有完成源，并且任务是一次性或已执行次数达到重复次数，则设置完成源
            if (task.CompletionSource != null && (task.IsOnce || task.ExecutedCount >= task.RepeatCount))
            {
                // 设置完成源为成功
                task.CompletionSource.TrySetResult();
                task.CompletionSource = null;
            }
        }

        /// <summary>
        /// 标记任务为取消
        /// </summary>
        /// <param name="task"></param>
        private void MarkCancelled(TimerTask task)
        {
            if (task.IsCancelled)
            {
                return;
            }

            task.IsCancelled = true;
            task.Callback = null;

            if (task.CompletionSource != null)
            {
                task.CompletionSource.TrySetCanceled();
                task.CompletionSource = null;
            }
        }

        /// <summary>
        /// 尝试获取任务
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool TryGetTask(TimerHandle handle, out TimerTask task)
        {
            task = null;
            if (handle.Id <= 0)
            {
                return false;
            }

            if (!_taskMap.TryGetValue(handle.Id, out task))
            {
                return false;
            }

            if (task == null || task.IsCancelled || task.Version != handle.Version)
            {
                task = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 刷新已移除的任务
        /// </summary>
        private void FlushRemoved()
        {
            if (_pendingRemoveIds.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _pendingRemoveIds.Count; i++)
            {
                int id = _pendingRemoveIds[i];
                if (!_taskMap.TryGetValue(id, out TimerTask task))
                {
                    continue;
                }

                _taskMap.Remove(id);

                int index = _activeTasks.IndexOf(task);
                if (index >= 0)
                {
                    int last = _activeTasks.Count - 1;
                    _activeTasks[index] = _activeTasks[last];
                    _activeTasks.RemoveAt(last);
                }

                ReturnTask(task);
            }

            _pendingRemoveIds.Clear();
        }

        /// <summary>
        /// 租借任务
        /// </summary>
        /// <returns></returns>
        private TimerTask RentTask()
        {
            return _taskPool.Count > 0 ? _taskPool.Pop() : new TimerTask();
        }

        /// <summary>
        /// 归还任务
        /// </summary>
        /// <param name="task"></param>
        private void ReturnTask(TimerTask task)
        {
            if (task == null)
            {
                return;
            }

            // 保留 Version，避免句柄复用误伤
            int version = task.Version;
            task.Reset();
            task.Version = version;
            _taskPool.Push(task);
        }
    }
}
