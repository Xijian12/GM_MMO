using Common;
using TMPro;
using UnityEngine;

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

        // TODO 创建服务器列表项
        private void CreateServerListItem()
        {

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
