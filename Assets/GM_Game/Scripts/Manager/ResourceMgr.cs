using System;
using System.Collections.Generic;
using Common;
using GM;
using UnityEngine;
using YooAsset;

namespace Manager
{
	/**
 	* Title:资源管理器
 	* Desciption:
 	**/
	public class ResourceMgr : Singleton<ResourceMgr>
    {
    	// TODO 这里需要该用对象池来缓存资源
        private readonly Dictionary<string, AssetOperationHandle> prefabCacheDic = new Dictionary<string, AssetOperationHandle>();
        /// <summary>
        /// 异步加载Prefab资源
        /// </summary>
        /// <param name="path">资源路径，不包含Prefab路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadAssetAsync<T>(string path, Action<GameObject> callback) where T : UnityEngine.Object
        {
        	if(prefabCacheDic.ContainsKey(path))
        	{
        		callback?.Invoke(prefabCacheDic[path].InstantiateSync());
        		return;
        	}
            Global.Instance.YooPackage.LoadAssetAsync<T>(ConstDefine.PREFAB_PATH + path)
            .Completed += (AssetOperationHandle handle) =>
            {
                GameObject go = handle.InstantiateSync();
                prefabCacheDic.Add(path, handle);
                callback?.Invoke(go);
            };
        }
	}
}
