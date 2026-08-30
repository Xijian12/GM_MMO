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

        // 登录信息
        public LoginRet LoginInfo { get; set; }

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
            // 启动闲置实例 TTL 扫描（仅清实例，不 Release Handle）
            GameObjectPoolMgr.Instance.StartIdleAssetCleanupLoop(_appCts.Token);
            // 启动闲置 Handle TTL 扫描
            ResourceMgr.Instance.StartHandleCleanupLoop(_appCts.Token);

            // 初始化定时器管理器（挂在 Global 下，随 DontDestroyOnLoad 跨场景存活）
            Transform timerRoot = new GameObject("TimerRoot").transform;
            timerRoot.SetParent(transform, false);
            TimerMgr.Instance.Initialize(timerRoot);

            // 初始化网络模块
            NetSocketMgr.Instance.Init();
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

        /// <summary>
        /// 当程序退出的时候，断开与服务端的连接
        /// </summary>
        private void OnApplicationQuit()
        {
            NetSocketMgr.Instance.DisConnect();
        }
    }
}
