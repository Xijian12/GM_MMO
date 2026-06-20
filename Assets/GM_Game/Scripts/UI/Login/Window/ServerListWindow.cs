using Common;
using TMPro;
using UnityEngine;
using YooAsset;

/**
 * Title:服务器列表窗口
 * Desciption:
 **/
namespace UI.Login
{
    public class ServerListWindow : UIBase
    {
        [SerializeField, Header("服务器名称")] private TMP_Text _textServerName;
        [SerializeField, Header("服务器列表父节点")] private Transform _itemParentTrans;
        [SerializeField, Header("关闭按钮")] private UGUIBtn _btnClose;

        public override void InitView()
        {
            _btnClose.AddClick(OnCloseBtnClicked);
        }

        /// <summary>
        /// 初始化服务器列表窗口
        /// </summary>
        private void Start()
        {
            CreateServerListItem();
        }

        // TODO 创建服务器列表项
        private void CreateServerListItem()
        {
            for (int i = 0; i < 50; i++)
            {
                Global.Instance.Yoopackage.LoadAssetAsync<GameObject>("Assets/GM_Game/Prefabs/UIPrefabs/ServerListWidgetItem")
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

        private void OnCloseBtnClicked()
        {
            UIRoot.Instance.LoginCtrl.ShowWindow(WindowType.GameServerWindow);
        }

        public void OnDestroy()
        {
            _btnClose.RemoveClick(OnCloseBtnClicked);
        }
    }
}
