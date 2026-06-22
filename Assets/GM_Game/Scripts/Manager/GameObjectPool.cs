using System.Collections.Generic;
using Common;
using UnityEngine;
using YooAsset;

namespace Manager
{
    /**
     * Title:单个 Prefab 路径的 GameObject 对象池
     * Desciption:复用已实例化的 GameObject，不负责 YooAsset Handle 的加载与释放。
     **/
    public class GameObjectPool
    {
        private readonly string _path;
        private readonly Transform _poolRoot; // 对象池根节点
        private readonly int _maxIdle; // 最大闲置对象数量
        private readonly Stack<GameObject> _idleInstances = new Stack<GameObject>(); // 闲置对象池

        public string Path => _path; // 资源路径
        public int ActiveCount { get; private set; } // 活跃对象数量
        public int IdleCount => _idleInstances.Count; // 闲置对象数量
        public float LastAccessTime { get; private set; } // 最后一次访问时间
        public bool CanReleaseAsset => ActiveCount == 0 && IdleCount == 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="poolRoot">对象池根节点</param>
        /// <param name="maxIdle">最大闲置对象数量</param>
        public GameObjectPool(string path, Transform poolRoot, int maxIdle)
        {
            _path = path;
            _poolRoot = poolRoot;
            _maxIdle = maxIdle;
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="handle">资源句柄</param>
        /// <param name="parent">父节点</param>
        /// <returns>获取到的对象</returns>
        public GameObject Spawn(AssetOperationHandle handle, Transform parent)
        {
            // 尝试从闲置对象池中获取对象
            GameObject go = TryPopIdle(parent);
            // 如果闲置对象池中没有对象，则实例化新对象
            if (go == null)
            {
                go = handle.InstantiateSync(parent);
                AttachPooledObject(go);
            }

            ActiveCount++;
            TouchAccess();
            NotifySpawn(go);
            return go;
        }

        /// <summary>
        /// 将对象归还到对象池
        /// </summary>
        /// <param name="go">要归还的对象</param>
        public void Despawn(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            NotifyDespawn(go);
            go.SetActive(false);
            go.transform.SetParent(_poolRoot, false);

            // 如果闲置对象数量大于等于最大闲置对象数量，则销毁对象
            if (_maxIdle > 0 && _idleInstances.Count >= _maxIdle)
            {
                Object.Destroy(go); // 销毁对象
            }
            else
            {
                _idleInstances.Push(go); // 将对象放回闲置对象池
            }

            ActiveCount = Mathf.Max(0, ActiveCount - 1); // 活跃对象数量减少
            TouchAccess();
        }

        /// <summary>
        /// 清空闲置实例并重置计数（用于离场景时，假定活跃实例随场景卸载销毁）。
        /// </summary>
        public void ClearAndReset()
        {
            Clear();
            ActiveCount = 0;
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            while (_idleInstances.Count > 0)
            {
                GameObject go = _idleInstances.Pop();
                if (go == null)
                {
                    continue;
                }

                NotifyDespawn(go);
                Object.Destroy(go);
            }
        }

        /// <summary>
        /// 触摸访问时间
        /// </summary>
        private void TouchAccess()
        {
            LastAccessTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 尝试从闲置对象池中获取对象
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <returns>获取到的对象</returns>
        private GameObject TryPopIdle(Transform parent)
        {
            while (_idleInstances.Count > 0)
            {
                GameObject go = _idleInstances.Pop();
                if (go == null)
                {
                    continue;
                }

                Transform goTransform = go.transform;
                goTransform.SetParent(parent, false);
                go.SetActive(true);
                return go;
            }

            return null;
        }

        /// <summary>
        /// 挂载池化对象标记
        /// </summary>
        /// <param name="go">要挂载的对象</param>
        private void AttachPooledObject(GameObject go)
        {
            PooledObject pooledObject = go.GetComponent<PooledObject>();
            if (pooledObject == null)
            {
                pooledObject = go.AddComponent<PooledObject>();
            }

            pooledObject.Path = _path;
        }

        /// <summary>
        /// 通知对象池化对象激活
        /// </summary>
        /// <param name="go">要通知的对象</param>
        private static void NotifySpawn(GameObject go)
        {
            IPoolable[] poolables = go.GetComponents<IPoolable>();
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnSpawn();
            }
        }

        /// <summary>
        /// 通知对象池化对象回收
        /// </summary>
        /// <param name="go">要通知的对象</param>
        private static void NotifyDespawn(GameObject go)
        {
            IPoolable[] poolables = go.GetComponents<IPoolable>();
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnDespawn();
            }
        }
    }
}
