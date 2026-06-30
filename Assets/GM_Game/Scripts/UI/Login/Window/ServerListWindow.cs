using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf.Collections;
using Manager;
using TMPro;
using UI;
using UnityEngine;
using YooAsset;

namespace UI.Login
{
    /**
     * Title:服务器列表窗口
     * Desciption:
     **/
    public class ServerListWindow : WindowBase
    {
        [SerializeField, Header("服务器名称")] private TMP_Text _textServerName;
        [SerializeField, Header("服务器列表父节点")] private Transform _itemParentTrans;
        [SerializeField, Header("关闭按钮")] private UGUIBtn _btnClose;

        private const string ServerListItemPath = "UIPrefabs/ServerListWidgetItem";

        private readonly List<GameObject> _spawnedItems = new List<GameObject>(32);
        private CancellationTokenSource _refreshCts;
        private RepeatedField<GameServer> _gameServers;

        public override void InitView()
        {
            _btnClose.AddClick(OnCloseBtnClicked);
        }

        /// <summary>
        /// 设置并刷新服务器列表（类型安全入口，供 LoginView 调用）。
        /// </summary>
        public void SetServerList(GetServerListRet ret)
        {
            RefreshUI(ret);
        }

        public override void RefreshUI(object obj)
        {
            if (obj is not GetServerListRet ret || ret.GameServers == null || ret.GameServers.Count == 0)
            {
                return;
            }

            // 如果存在缓存数据，且缓存数据和新获取的数据一样，则直接返回
            if (_gameServers != null && _gameServers.SequenceEqual(ret.GameServers))
            {
                return;
            }

            _gameServers = ret.GameServers;
            RefreshServerListItemsAsync().Forget();
        }

        public override void Hide()
        {
            CancelRefresh();
            base.Hide();
        }

        /// <summary>
        /// 异步创建/刷新服务器列表项。
        /// </summary>
        private async UniTaskVoid RefreshServerListItemsAsync()
        {
            // 取消当前的刷新任务
            CancelRefresh();
            _refreshCts = new CancellationTokenSource();
            CancellationToken token = _refreshCts.Token;

            // 清空当前的列表项
            ClearItems();

            // 加载服务器列表项预制体
            AssetOperationHandle handle = await ResourceMgr.Instance.GetPrefabHandleAsync(ServerListItemPath, token);
            if (handle == null)
            {
                Debug.LogError("[ServerListWindow] 加载 ServerListWidgetItem 失败。");
                return;
            }

            // 创建服务器列表项
            try
            {
                for (int i = 0; i < _gameServers.Count; i++)
                {
                    // 如果取消，则抛出异常
                    token.ThrowIfCancellationRequested();

                    // 实例化服务器列表项预制体
                    GameObject go = handle.InstantiateSync(_itemParentTrans);
                    // 设置缩放为1
                    go.transform.localScale = Vector3.one;
                    go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    if (go.TryGetComponent(out GameServerItem item))
                    {
                        item.RefreshUI(_gameServers[i]);
                    }

                    _spawnedItems.Add(go);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 销毁当前列表项。
        /// </summary>
        private void ClearItems()
        {
            for (int i = 0; i < _spawnedItems.Count; i++)
            {
                if (_spawnedItems[i] != null)
                {
                    Destroy(_spawnedItems[i]);
                }
            }

            _spawnedItems.Clear();
        }

        /// <summary>
        /// 取消进行中的刷新任务。
        /// </summary>
        private void CancelRefresh()
        {
            if (_refreshCts == null)
            {
                return;
            }

            _refreshCts.Cancel();
            _refreshCts.Dispose();
            _refreshCts = null;
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void OnCloseBtnClicked()
        {
            UIRoot.Instance.LoginViewCtrl.ShowWindow(WindowType.GameServerWindow);
        }

        public void OnDestroy()
        {
            CancelRefresh();
            ClearItems();
            _btnClose.RemoveClick(OnCloseBtnClicked);
        }
    }
}
