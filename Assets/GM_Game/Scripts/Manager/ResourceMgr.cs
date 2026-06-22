using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using GM;
using UnityEngine;
using YooAsset;

namespace Manager
{
    /**
 	* Title:资源管理器
 	* Desciption:缓存 YooAsset Prefab Handle，供对象池或业务直接实例化。
 	**/
    public class ResourceMgr : Singleton<ResourceMgr>
    {
        // 缓存已加载的 Prefab Handle
        private readonly Dictionary<string, AssetOperationHandle> _prefabHandleCacheDic = new Dictionary<string, AssetOperationHandle>();
        // 缓存正在加载的 Prefab Handle
        private readonly Dictionary<string, UniTask<AssetOperationHandle>> _loadingTasks = new Dictionary<string, UniTask<AssetOperationHandle>>();
        // 缓存资源最后一次访问时间
        private readonly Dictionary<string, float> _lastAccessTimes = new Dictionary<string, float>();

        /// <summary>
        /// 获取已缓存的 Prefab Handle，供对象池实例化使用。
		/// 去重/单飞 防止同一资源重复加载。
        /// </summary>
        public async UniTask<AssetOperationHandle> GetPrefabHandleAsync(string path, CancellationToken cancellationToken = default)
        {
            // 尝试从缓存中获取已加载的 Prefab Handle
            if (_prefabHandleCacheDic.TryGetValue(path, out AssetOperationHandle cachedHandle))
            {
                TouchAccess(path);
                return cachedHandle;
            }

            // 尝试从加载任务中获取已加载的 Prefab Handle
            if (_loadingTasks.TryGetValue(path, out UniTask<AssetOperationHandle> loadingTask))
            {
                Debug.LogError("使用缓存资源handle：" + path);
                return await loadingTask.AttachExternalCancellation(cancellationToken);
            }

            // 创建新的加载任务(第一次加载时创建)
            UniTask<AssetOperationHandle> task = LoadPrefabHandleInternalAsync(path);
            _loadingTasks[path] = task;

            Debug.LogError("创建新的加载任务：" + path);

            try
            {
                // 等待加载完成并返回 AssetOperationHandle
                return await task.AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                // 移除加载任务
                _loadingTasks.Remove(path);
            }
        }

        /// <summary>
        /// 异步加载并实例化 Prefab（不走对象池）。
        /// </summary>
        /// <param name="path">资源路径，不包含 Prefab 根路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadAssetAsync<T>(string path, Action<GameObject> callback) where T : UnityEngine.Object
        {
            LoadAssetAsyncInternal<T>(path, callback).Forget();
        }

        /// <summary>
        /// 尝试获取已缓存的 Prefab Handle。
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="handle">获取到的 AssetOperationHandle</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetCachedHandle(string path, out AssetOperationHandle handle)
        {
            return _prefabHandleCacheDic.TryGetValue(path, out handle);
        }

        /// <summary>
        /// 释放 Prefab Handle。
        /// </summary>
        /// <param name="path">资源路径</param>
        public void ReleasePrefabHandle(string path)
        {
            if (!_prefabHandleCacheDic.TryGetValue(path, out AssetOperationHandle handle))
            {
                return;
            }

            // 释放 Prefab Handle
            handle.Release();
            // 移除缓存
            _prefabHandleCacheDic.Remove(path);
            // 移除资源最后一次访问时间
            _lastAccessTimes.Remove(path);
        }

        /// <summary>
        /// 释放所有已缓存的 Prefab Handle。
        /// </summary>
        public void ReleaseAll()
        {
            // 释放所有已缓存的 Prefab Handle
            foreach (AssetOperationHandle handle in _prefabHandleCacheDic.Values)
            {
                handle.Release();
            }

            _prefabHandleCacheDic.Clear();
            _lastAccessTimes.Clear();
        }

        /// <summary>
        /// 获取当前已缓存的资源路径快照。
		/// <param name="results">结果列表</param>
        /// </summary>
        public void CopyCachedPaths(List<string> results)
        {
            results.Clear();
            foreach (string path in _prefabHandleCacheDic.Keys)
            {
                results.Add(path);
            }
        }

        /// <summary>
        /// 获取资源最后一次访问时间（Time.realtimeSinceStartup）。
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="lastAccessTime">最后一次访问时间</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetLastAccessTime(string path, out float lastAccessTime)
        {
            return _lastAccessTimes.TryGetValue(path, out lastAccessTime);
        }

        /// <summary>
        /// 更新资源访问时间。
        /// </summary>
        public void TouchAccess(string path)
        {
            _lastAccessTimes[path] = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 异步加载并实例化 Prefab（不走对象池）。
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        private async UniTaskVoid LoadAssetAsyncInternal<T>(string path, Action<GameObject> callback) where T : UnityEngine.Object
        {
            AssetOperationHandle handle = await GetPrefabHandleAsync(path);
            if (handle == null)
            {
                callback?.Invoke(null);
                return;
            }

            GameObject go = handle.InstantiateSync();
            callback?.Invoke(go);
        }

        /// <summary>
        /// 加载 Prefab Handle
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>加载完成的 AssetOperationHandle</returns>
        private async UniTask<AssetOperationHandle> LoadPrefabHandleInternalAsync(string path)
        {
            string assetPath = ConstDefine.PREFAB_PATH + path;
            AssetOperationHandle handle = Global.Instance.YooPackage.LoadAssetAsync<GameObject>(assetPath);
            await AwaitHandleAsync(handle);

            if (handle.Status != EOperationStatus.Succeed)
            {
                Debug.LogError($"[ResourceMgr] 加载 Prefab 失败: {assetPath}, {handle.LastError}");
                return null;
            }

            _prefabHandleCacheDic[path] = handle;
            TouchAccess(path);
            return handle;
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
