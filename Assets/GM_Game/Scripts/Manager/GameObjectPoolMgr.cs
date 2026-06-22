using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Manager
{
    /**
     * Title:GameObject 对象池管理器
     * Desciption:按 Prefab 路径管理多个 GameObjectPool，依赖 ResourceMgr 获取 YooAsset Handle。
     **/
    public class GameObjectPoolMgr : Singleton<GameObjectPoolMgr>
    {
        private const int DefaultMaxIdle = 10;
        private const float CleanupIntervalSeconds = 30f;
        private const float IdleAssetTtlSeconds = 30f;

        private Transform _poolRoot;
        private readonly Dictionary<string, GameObjectPool> _pools = new Dictionary<string, GameObjectPool>();
        private readonly List<string> _cachedPathBuffer = new List<string>(32);

        public bool IsInitialized => _poolRoot != null;

        /// <summary>
        /// 由 Global 在启动时注入池根节点（挂于 DontDestroyOnLoad 物体下）。
        /// </summary>
        public void Initialize(Transform poolRoot)
        {
            if (poolRoot == null)
            {
                throw new ArgumentNullException(nameof(poolRoot));
            }

            _poolRoot = poolRoot;
        }

        /// <summary>
        /// 异步获取对象池中的对象。
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="parent">父节点</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="maxIdle">最大闲置对象数量</param>
        /// <returns>获取到的对象</returns>
        public async UniTask<GameObject> SpawnAsync(
            string path,
            Transform parent,
            CancellationToken cancellationToken = default,
            int maxIdle = DefaultMaxIdle)
        {
            if (!IsInitialized)
            {
                Debug.LogError("[GameObjectPoolMgr] 未初始化，请先调用 Initialize。");
                return null;
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[GameObjectPoolMgr] SpawnAsync path 为空。");
                return null;
            }

            GameObjectPool pool = GetOrCreatePool(path, maxIdle);
            ResourceMgr.Instance.TouchAccess(path);
            // 异步获取 Prefab Handle
            AssetOperationHandle handle = await ResourceMgr.Instance.GetPrefabHandleAsync(path, cancellationToken);
            // 如果获取到的 Prefab Handle 为空，则返回 null
            if (handle == null)
            {
                return null;
            }

            return pool.Spawn(handle, parent);
        }

        /// <summary>
        /// 归还对象池中的对象。
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="go">要归还的对象</param>
        public void Despawn(string path, GameObject go)
        {
            if (go == null || string.IsNullOrEmpty(path))
            {
                return;
            }

            if (_pools.TryGetValue(path, out GameObjectPool pool))
            {
                pool.Despawn(go);
            }
            else
            {
                Debug.LogWarning($"[GameObjectPoolMgr] Despawn 时未找到池: {path}，将直接销毁。");
                UnityEngine.Object.Destroy(go);
            }
        }

        /// <summary>
        /// 通过 PooledObject 标记归还，无需再传 path。
        /// </summary>
        public void Despawn(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            PooledObject pooledObject = go.GetComponent<PooledObject>();
            // 如果 PooledObject 标记为空或路径为空，则直接销毁。
            if (pooledObject == null || string.IsNullOrEmpty(pooledObject.Path))
            {
                Debug.LogWarning("[GameObjectPoolMgr] 对象缺少 PooledObject 标记，将直接销毁。");
                UnityEngine.Object.Destroy(go);
                return;
            }

            // 通过 PooledObject 标记归还，无需再传 path。
            Despawn(pooledObject.Path, go);
        }

        /// <summary>
        /// 清空指定对象池
        /// </summary>
        /// <param name="path">资源路径</param>
        public void ClearPool(string path)
        {
            if (_pools.TryGetValue(path, out GameObjectPool pool))
            {
                pool.Clear();
                _pools.Remove(path);
            }
        }

        /// <summary>
        /// 清空所有对象池并重置计数（离场景时调用）。
        /// </summary>
        public void ClearAllAndReset()
        {
            foreach (GameObjectPool pool in _pools.Values)
            {
                pool.ClearAndReset();
            }

            _pools.Clear();
        }

        /// <summary>
        /// 尝试获取指定路径的对象池。
        /// </summary>
        public bool TryGetPool(string path, out GameObjectPool pool)
        {
            return _pools.TryGetValue(path, out pool);
        }

        /// <summary>
        /// 启动闲置资源 TTL 扫描（由 Global 在启动时调用）。
        /// </summary>
        public void StartIdleAssetCleanupLoop(CancellationToken cancellationToken)
        {
            IdleAssetCleanupLoopAsync(cancellationToken).Forget();
        }

        /// <summary>
        /// 扫描并释放长时间未使用且无实例占用的 Prefab Handle。
        /// </summary>
        /// <param name="idleTtlSeconds">闲置对象 TTL 时间</param>
        public void TryEvictUnusedAssets(float idleTtlSeconds)
        {
            float now = Time.realtimeSinceStartup;
            ResourceMgr resourceMgr = ResourceMgr.Instance;
            resourceMgr.CopyCachedPaths(_cachedPathBuffer);

            // 遍历所有缓存的资源路径
            for (int i = 0; i < _cachedPathBuffer.Count; i++)
            {
                string path = _cachedPathBuffer[i];

                // 尝试获取对应路径的对象池
                if (_pools.TryGetValue(path, out GameObjectPool pool))
                {
                    // 如果对象池中还有活跃对象，则跳过
                    if (!pool.CanReleaseAsset)
                    {
                        continue;
                    }

                    if (now - pool.LastAccessTime < idleTtlSeconds)
                    {
                        continue;
                    }
                }
                // 如果获取不到对应路径的对象池，则尝试获取对应路径的 Prefab Handle 的最后一次访问时间
                else if (resourceMgr.TryGetLastAccessTime(path, out float lastAccessTime))
                {
                    // 如果最后一次访问时间小于闲置对象 TTL 时间，则跳过
                    if (now - lastAccessTime < idleTtlSeconds)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                ClearPool(path);
                resourceMgr.ReleasePrefabHandle(path);
            }
        }

        /// <summary>
        /// 启动闲置资源 TTL 扫描（由 Global 在启动时调用）。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        private async UniTaskVoid IdleAssetCleanupLoopAsync(CancellationToken cancellationToken)
        {
            // 循环扫描并释放长时间未使用且无实例占用的 Prefab Handle
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // 延迟一段时间后再次扫描
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(CleanupIntervalSeconds),
                        cancellationToken: cancellationToken);
                    // 扫描并释放长时间未使用且无实例占用的 Prefab Handle
                    TryEvictUnusedAssets(IdleAssetTtlSeconds);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 获取或创建对象池
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="maxIdle">最大闲置对象数量</param>
        /// <returns>获取到的对象池</returns>
        private GameObjectPool GetOrCreatePool(string path, int maxIdle)
        {
            if (!_pools.TryGetValue(path, out GameObjectPool pool))
            {
                pool = new GameObjectPool(path, _poolRoot, maxIdle);
                _pools.Add(path, pool);
            }

            return pool;
        }
    }
}
