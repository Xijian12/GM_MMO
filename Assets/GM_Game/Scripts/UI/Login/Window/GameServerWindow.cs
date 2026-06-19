using UnityEngine;
using Common;

namespace UI.Login
{

    /**
     * Title:登录服务器窗口
     * Desciption:
     **/
    public class GameServerWindow : UIBase
    {
        [SerializeField, Header("跳转服务器列表按钮")] private UGUIBtn _btnGotoServerList;

        public override void InitView()
        {
            _btnGotoServerList.AddClick(OnGotoServerListBtnClicked);
        }

        // 跳转服务器列表按钮点击事件
        public void OnGotoServerListBtnClicked()
        {
            UIRoot.Instance.LoginCtrl.ShowWindow(WindowType.ServerListWindow);
        }

        // TODO 游戏服务器按钮点击事件
        public void OnGameServerBtnClicked()
        {
        }

        public void OnDestroy()
        {
            _btnGotoServerList.RemoveClick(OnGotoServerListBtnClicked);
        }

    }
}