using Common;
using GM;
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

        public override void InitView()
        {
            _btnClose.AddClick(OnCloseBtnClicked);
        }

        private void Start()
        {
            CreateServerListItem();
        }

        /// <summary>
        /// 创建服务器列表项
        /// </summary>
        private void CreateServerListItem()
        {
            for (int i = 0; i < 50; i++)
            {
                Global.Instance.YooPackage.LoadAssetAsync<GameObject>("Assets/GM_Game/Prefabs/UIPrefabs/ServerListWidgetItem")
                    .Completed += (AssetOperationHandle handle) =>
                    {
                        GameObject go = handle.InstantiateSync();
                        go.transform.SetParent(_itemParentTrans);
                        go.transform.localScale = Vector3.one;
                        go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                        go.SetActive(true);
                    };
            }
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
            _btnClose.RemoveClick(OnCloseBtnClicked);
        }
    }
}
