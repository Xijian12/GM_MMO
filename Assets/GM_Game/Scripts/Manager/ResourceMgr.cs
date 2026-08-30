using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using GM;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace Manager
{
    /**
 	* Title:资源管理器
 	* Desciption:通用 Unity 资产加载入口；Handle 缓存与去重；Prefab 可经 usePool 走 GameObjectPoolMgr。
 	**/
    public class ResourceMgr : Singleton<ResourceMgr>
    {
        private const float CleanupIntervalSeconds = 30f;
        private const float IdleHandleTtlSeconds = 30f;

        // 缓存已加载的 Handle（key = ResourceType|shortPath）
        private readonly Dictionary<string, AssetOperationHandle> _handleCache = new Dictionary<string, AssetOperationHandle>();
        // 缓存正在加载的任务（Preserve 后可多处 await）
        private readonly Dictionary<string, UniTask<AssetOperationHandle>> _loadingTasks = new Dictionary<string, UniTask<AssetOperationHandle>>();
        // 缓存资源最后一次访问时间
        private readonly Dictionary<string, float> _lastAccessTimes = new Dictionary<string, float>();
        private readonly List<string> _cacheKeyBuffer = new List<string>(32);

        /// <summary>
        /// 构建缓存 Key。
        /// </summary>
        public static string BuildCacheKey(ResourceType type, string shortPath)
        {
            return $"{type}|{shortPath}";
        }

        /// <summary>
        /// 按类型拼完整资产路径。
        /// </summary>
        public static string ResolveAssetPath(ResourceType type, string shortPath)
        {
            string root = type switch
            {
                ResourceType.Prefab => ConstDefine.PREFAB_PATH,
                ResourceType.Texture => ConstDefine.TEXTURE_PATH,
                ResourceType.Sprite => ConstDefine.SPRITE_PATH,
                ResourceType.Audio => ConstDefine.AUDIO_PATH,
                ResourceType.Material => ConstDefine.MATERIAL_PATH,
                ResourceType.Scriptable => ConstDefine.SCRIPTABLE_PATH,
                ResourceType.TextAsset => ConstDefine.TEXT_PATH,
                _ => ConstDefine.BASE_PATH
            };
            return root + shortPath;
        }

        /// <summary>
        /// 通用异步加载资产本体（缓存 Handle）。
        /// </summary>
        public async UniTask<T> LoadAssetAsync<T>(
            ResourceType type,
            string shortPath,
            CancellationToken cancellationToken = default)
            where T : Object
        {
            AssetOperationHandle handle = await GetHandleAsync(type, shortPath, cancellationToken);
            if (handle == null)
            {
                return null;
            }

            return handle.AssetObject as T;
        }

        /// <summary>
        /// 异步获取 Prefab 实例。
        /// usePool=true 时 RegisterTemplate + 出池；false 时 InstantiateSync 且不入池。
        /// </summary>
        public async UniTask<GameObject> SpawnPrefabAsync(
            string shortPath,
            Transform parent,
            bool usePool = true,
            CancellationToken cancellationToken = default,
            int maxIdle = 10)
        {
            AssetOperationHandle handle = await GetHandleAsync(
                ResourceType.Prefab, shortPath, cancellationToken);
            if (handle == null)
            {
                return null;
            }

            string key = BuildCacheKey(ResourceType.Prefab, shortPath);

            if (usePool)
            {
                if (!GameObjectPoolMgr.Instance.IsInitialized)
                {
                    Debug.LogError("[ResourceMgr] GameObjectPoolMgr 未初始化，无法 usePool。");
                    return null;
                }

                GameObjectPoolMgr.Instance.RegisterTemplate(key, handle, maxIdle);
                return GameObjectPoolMgr.Instance.Spawn(key, parent);
            }

            return handle.InstantiateSync(parent);
        }

        /// <summary>
        /// 归还池化实例，或 Destroy 非池化实例。
        /// </summary>
        public void Recycle(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            PooledObject pooled = go.GetComponent<PooledObject>();
            if (pooled != null && !string.IsNullOrEmpty(pooled.Path))
            {
                GameObjectPoolMgr.Instance.Despawn(go);
                return;
            }

            Object.Destroy(go);
        }

        /// <summary>
        /// 释放指定资源 Handle（Prefab 须无实例占用）。
        /// </summary>
        public void Release(ResourceType type, string shortPath)
        {
            if (string.IsNullOrEmpty(shortPath))
            {
                return;
            }

            string key = BuildCacheKey(type, shortPath);
            if (type == ResourceType.Prefab && !GameObjectPoolMgr.Instance.CanReleaseHandle(key))
            {
                Debug.LogWarning($"[ResourceMgr] 仍有实例占用，拒绝 Release: {key}");
                return;
            }

            if (!_handleCache.TryGetValue(key, out AssetOperationHandle handle))
            {
                return;
            }

            if (type == ResourceType.Prefab)
            {
                GameObjectPoolMgr.Instance.ClearPool(key);
            }

            handle.Release();
            _handleCache.Remove(key);
            _lastAccessTimes.Remove(key);
        }

        /// <summary>
        /// 清空对象池并释放全部已缓存 Handle。
        /// </summary>
        public void ReleaseAll()
        {
            GameObjectPoolMgr.Instance.ClearAllAndReset();

            foreach (AssetOperationHandle handle in _handleCache.Values)
            {
                handle.Release();
            }

            _handleCache.Clear();
            _lastAccessTimes.Clear();
        }

        /// <summary>
        /// 更新资源访问时间。
        /// </summary>
        public void TouchAccess(ResourceType type, string shortPath)
        {
            if (string.IsNullOrEmpty(shortPath))
            {
                return;
            }

            string key = BuildCacheKey(type, shortPath);
            _lastAccessTimes[key] = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 启动闲置 Handle TTL 扫描（由 Global 在启动时调用）。
        /// </summary>
        public void StartHandleCleanupLoop(CancellationToken cancellationToken)
        {
            HandleCleanupLoopAsync(cancellationToken).Forget();
        }

        /// <summary>
        /// 扫描并释放超时且无占用的 Handle。
        /// </summary>
        public void TryEvictUnusedHandles(float idleTtlSeconds)
        {
            float now = Time.realtimeSinceStartup;
            _cacheKeyBuffer.Clear();
            foreach (string key in _handleCache.Keys)
            {
                _cacheKeyBuffer.Add(key);
            }

            for (int i = 0; i < _cacheKeyBuffer.Count; i++)
            {
                string key = _cacheKeyBuffer[i];
                if (!_lastAccessTimes.TryGetValue(key, out float lastAccess))
                {
                    lastAccess = 0f;
                }

                if (now - lastAccess < idleTtlSeconds)
                {
                    continue;
                }

                if (!TryParseCacheKey(key, out ResourceType type, out string shortPath))
                {
                    continue;
                }

                if (type == ResourceType.Prefab)
                {
                    if (!GameObjectPoolMgr.Instance.CanReleaseHandle(key))
                    {
                        continue;
                    }

                    // 池若仍有 lastAccess，以池为准再判一次
                    if (GameObjectPoolMgr.Instance.TryGetPoolState(key, out _, out _, out float poolLastAccess)
                        && now - poolLastAccess < idleTtlSeconds)
                    {
                        continue;
                    }
                }

                Release(type, shortPath);
            }
        }

        private async UniTask<AssetOperationHandle> GetHandleAsync(
            ResourceType type,
            string shortPath,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(shortPath))
            {
                Debug.LogError("[ResourceMgr] shortPath 为空。");
                return null;
            }

            string key = BuildCacheKey(type, shortPath);
            if (_handleCache.TryGetValue(key, out AssetOperationHandle cachedHandle))
            {
                TouchAccess(type, shortPath);
                return cachedHandle;
            }

            // 正在加载：await 已 Preserve 的同一 UniTask（UniTask 默认不能多次 await）
            if (_loadingTasks.TryGetValue(key, out UniTask<AssetOperationHandle> loadingTask))
            {
                return await loadingTask.AttachExternalCancellation(cancellationToken);
            }

            // 创建新的加载任务；Preserve 允许多个调用方同时等待同一 key
            UniTask<AssetOperationHandle> task = LoadHandleInternalAsync(type, shortPath, key).Preserve();
            _loadingTasks[key] = task;

            try
            {
                return await task.AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                _loadingTasks.Remove(key);
            }
        }

        private async UniTask<AssetOperationHandle> LoadHandleInternalAsync(
            ResourceType type,
            string shortPath,
            string key)
        {
            string assetPath = ResolveAssetPath(type, shortPath);
            AssetOperationHandle handle = Global.Instance.YooPackage.LoadAssetAsync(assetPath);
            await AwaitHandleAsync(handle);

            if (handle.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"[ResourceMgr] 加载失败: {assetPath}, {handle.LastError}");
                return null;
            }

            _handleCache[key] = handle;
            TouchAccess(type, shortPath);
            return handle;
        }

        private async UniTaskVoid HandleCleanupLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(CleanupIntervalSeconds),
                        cancellationToken: cancellationToken);
                    TryEvictUnusedHandles(IdleHandleTtlSeconds);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private static bool TryParseCacheKey(string key, out ResourceType type, out string shortPath)
        {
            type = default;
            shortPath = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            int sep = key.IndexOf('|');
            if (sep <= 0 || sep >= key.Length - 1)
            {
                return false;
            }

            if (!Enum.TryParse(key.Substring(0, sep), out type))
            {
                return false;
            }

            shortPath = key.Substring(sep + 1);
            return true;
        }

        /// <summary>
        /// 等待 AssetOperationHandle 完成
        /// </summary>
        /// <param name="handle">AssetOperationHandle</param>
        /// <returns>等待完成的 UniTask</returns>
        private static UniTask AwaitHandleAsync(AssetOperationHandle handle)
        {
            if (handle.IsDone)
            {
                return UniTask.CompletedTask;
            }

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            handle.Completed += _ => completionSource.TrySetResult();
            return completionSource.Task;
        }
    }
}
