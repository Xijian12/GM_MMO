using System.Threading;
using Manager;
using UnityEngine;
using YooAsset;

namespace GM
{
    /**
     * Title:
     * Desciption:
     **/
    public class Global : MonoBehaviour
    {
        public static Global Instance { get; private set; }
        private ResourcePackage _package;
        private CancellationTokenSource _appCts;

        public ResourcePackage YooPackage { get => _package; }

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);

            _package = YooAssets.GetPackage("DefaultPackage");
            _appCts = new CancellationTokenSource();

            Transform poolRoot = new GameObject("PoolRoot").transform;
            poolRoot.SetParent(transform, false);
            // 初始化对象池管理器
            GameObjectPoolMgr.Instance.Initialize(poolRoot);
            // 启动闲置资源 TTL 扫描（由 Global 在启动时调用）。
            GameObjectPoolMgr.Instance.StartIdleAssetCleanupLoop(_appCts.Token);
        }

        private void OnDestroy()
        {
            if (_appCts == null)
            {
                return;
            }

            _appCts.Cancel();
            _appCts.Dispose();
            _appCts = null;
        }
    }
}
