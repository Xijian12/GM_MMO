using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Manager
{
    /**
     * Title:定时器任务（内部）
     * Desciption:由对象池复用，不对外暴露。
     **/
    internal sealed class TimerTask
    {
        public int Id;
        public int Version;
        public float Delay; // 延迟时间,单位:秒
        public float Elapsed; // 已执行时间,单位:秒
        /// <summary>
        /// -1 无限循环；1 执行一次；N 执行 N 次。
        /// </summary>
        public int RepeatCount; // 重复次数
        public int ExecutedCount; // 已执行次数 ，0 表示未执行，1 表示已执行，N 表示已执行 N 次
        public Action Callback; // 回调函数 
        public TimerType Type;
        public bool IsPaused; // 是否暂停
        public bool IsCancelled; // 是否取消
        public UnityEngine.Object Owner; // 所属对象
        public UniTaskCompletionSource CompletionSource; // 完成源
        public string DebugName;

        public bool IsOnce => RepeatCount == 1;
        public bool IsInfinite => RepeatCount < 0;

        public void Reset()
        {
            Id = 0;
            Version = 0;
            Delay = 0f;
            Elapsed = 0f;
            RepeatCount = 1;
            ExecutedCount = 0;
            Callback = null;
            Type = TimerType.GameTime;
            IsPaused = false;
            IsCancelled = false;
            Owner = null;
            CompletionSource = null;
            DebugName = null;
        }

        /// <summary>
        /// 判断所属对象是否已销毁
        /// </summary>
        /// <returns></returns>
        public bool IsOwnerDestroyed()
        {
            if (ReferenceEquals(Owner, null))
            {
                return false;
            }

            // Unity 重载 ==：已 Destroy 的对象表现为 null
            return Owner == null;
        }
    }
}
