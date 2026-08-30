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
     * Desciption:按 cacheKey 管理多个 GameObjectPool；仅负责实例出池/还池与闲置实例清理，不依赖 ResourceMgr。
     **/
    public class GameObjectPoolMgr : Singleton<GameObjectPoolMgr>
    {
        private const int DefaultMaxIdle = 10;
        private const float CleanupIntervalSeconds = 30f;
        private const float IdleInstanceTtlSeconds = 30f;

        private Transform _poolRoot;
        private readonly Dictionary<string, GameObjectPool> _pools = new Dictionary<string, GameObjectPool>();
        private readonly Dictionary<string, AssetOperationHandle> _templates = new Dictionary<string, AssetOperationHandle>();
        private readonly List<string> _poolKeyBuffer = new List<string>(32);

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
        /// 注册实例化模板（幂等：同一 cacheKey 重复调用覆盖/沿用，不新建第二套池）。
        /// </summary>
        /// <param name="cacheKey">缓存键，例如 Prefab|UIPrefabs/SystemTips</param>
        /// <param name="handle">已加载的 YooAsset Handle</param>
        /// <param name="maxIdle">最大闲置实例数</param>
        public void RegisterTemplate(string cacheKey, AssetOperationHandle handle, int maxIdle = DefaultMaxIdle)
        {
            if (string.IsNullOrEmpty(cacheKey) || handle == null)
            {
                Debug.LogError("[GameObjectPoolMgr] RegisterTemplate 参数无效。");
                return;
            }

            if (!IsInitialized)
            {
                Debug.LogError("[GameObjectPoolMgr] 未初始化，无法 RegisterTemplate。");
                return;
            }

            _templates[cacheKey] = handle;
            GetOrCreatePool(cacheKey, maxIdle);
        }

        /// <summary>
        /// 从对象池取出实例（须先 RegisterTemplate）。
        /// </summary>
        public GameObject Spawn(string cacheKey, Transform parent)
        {
            if (!IsInitialized)
            {
                Debug.LogError("[GameObjectPoolMgr] 未初始化。");
                return null;
            }

            if (string.IsNullOrEmpty(cacheKey))
            {
                Debug.LogError("[GameObjectPoolMgr] Spawn cacheKey 为空。");
                return null;
            }

            if (!_templates.TryGetValue(cacheKey, out AssetOperationHandle handle) || handle == null)
            {
                Debug.LogError($"[GameObjectPoolMgr] 未 RegisterTemplate: {cacheKey}");
                return null;
            }

            GameObjectPool pool = GetOrCreatePool(cacheKey, DefaultMaxIdle);
            return pool.Spawn(handle, parent);
        }

        /// <summary>
        /// 归还对象池中的对象。
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="go">要归还的对象</param>
        public void Despawn(string cacheKey, GameObject go)
        {
            if (go == null || string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (_pools.TryGetValue(cacheKey, out GameObjectPool pool))
            {
                pool.Despawn(go);
            }
            else
            {
                Debug.LogWarning($"[GameObjectPoolMgr] Despawn 时未找到池: {cacheKey}，将直接销毁。");
                UnityEngine.Object.Destroy(go);
            }
        }

        /// <summary>
        /// 通过 PooledObject 标记归还，无需再传 cacheKey。
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
        /// 清空指定对象池（仅销毁闲置实例并移除池条目；不 Release Handle）。
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        public void ClearPool(string cacheKey)
        {
            if (_pools.TryGetValue(cacheKey, out GameObjectPool pool))
            {
                pool.Clear();
                _pools.Remove(cacheKey);
            }

            _templates.Remove(cacheKey);
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
            _templates.Clear();
        }

        /// <summary>
        /// 尝试获取指定 cacheKey 的对象池。
        /// </summary>
        public bool TryGetPool(string cacheKey, out GameObjectPool pool)
        {
            return _pools.TryGetValue(cacheKey, out pool);
        }

        /// <summary>
        /// 查询池状态（无池返回 false）。
        /// </summary>
        public bool TryGetPoolState(string cacheKey, out int active, out int idle, out float lastAccess)
        {
            active = 0;
            idle = 0;
            lastAccess = 0f;
            if (!_pools.TryGetValue(cacheKey, out GameObjectPool pool))
            {
                return false;
            }

            active = pool.ActiveCount;
            idle = pool.IdleCount;
            lastAccess = pool.LastAccessTime;
            return true;
        }

        /// <summary>
        /// 该 cacheKey 上是否已无实例占用（无池视为可释放 Handle）。
        /// </summary>
        public bool CanReleaseHandle(string cacheKey)
        {
            if (!_pools.TryGetValue(cacheKey, out GameObjectPool pool))
            {
                return true;
            }

            return pool.CanReleaseAsset;
        }

        /// <summary>
        /// 启动闲置实例 TTL 扫描（仅清闲置实例，不 Release YooAsset Handle）。
        /// </summary>
        public void StartIdleAssetCleanupLoop(CancellationToken cancellationToken)
        {
            IdleInstanceCleanupLoopAsync(cancellationToken).Forget();
        }

        /// <summary>
        /// 扫描并销毁长时间未使用池中的闲置实例（池变为可释放后由 ResourceMgr 决定是否 Release Handle）。
        /// </summary>
        /// <param name="idleTtlSeconds">闲置 TTL 时间</param>
        public void TryEvictUnusedIdleInstances(float idleTtlSeconds)
        {
            float now = Time.realtimeSinceStartup;
            _poolKeyBuffer.Clear();
            foreach (string key in _pools.Keys)
            {
                _poolKeyBuffer.Add(key);
            }

            for (int i = 0; i < _poolKeyBuffer.Count; i++)
            {
                string cacheKey = _poolKeyBuffer[i];
                if (!_pools.TryGetValue(cacheKey, out GameObjectPool pool))
                {
                    continue;
                }

                if (pool.ActiveCount > 0)
                {
                    continue;
                }

                if (pool.IdleCount == 0)
                {
                    continue;
                }

                if (now - pool.LastAccessTime < idleTtlSeconds)
                {
                    continue;
                }

                // 仅清闲置实例并移除空池/模板登记；Handle 留给 ResourceMgr
                pool.Clear();
                _pools.Remove(cacheKey);
                _templates.Remove(cacheKey);
            }
        }

        /// <summary>
        /// 启动闲置实例 TTL 扫描。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        private async UniTaskVoid IdleInstanceCleanupLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(CleanupIntervalSeconds),
                        cancellationToken: cancellationToken);
                    TryEvictUnusedIdleInstances(IdleInstanceTtlSeconds);
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
        /// <param name="cacheKey">缓存键</param>
        /// <param name="maxIdle">最大闲置对象数量</param>
        /// <returns>获取到的对象池</returns>
        private GameObjectPool GetOrCreatePool(string cacheKey, int maxIdle)
        {
            if (!_pools.TryGetValue(cacheKey, out GameObjectPool pool))
            {
                pool = new GameObjectPool(cacheKey, _poolRoot, maxIdle);
                _pools.Add(cacheKey, pool);
            }

            return pool;
        }
    }
}
