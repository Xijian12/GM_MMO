using Common;
using Manager;
using UI;
using UnityEngine;

namespace UI.Login
{
    /**
     * Title:登录服务器窗口
     * Desciption:
     **/
    public class GameServerWindow : WindowBase
    {
        [SerializeField, Header("跳转服务器列表按钮")] private UGUIBtn _btnGotoServerList;
        [SerializeField, Header("登录服务器按钮")] private UGUIBtn _btnLoginServer;

        public override void InitView()
        {
            _btnGotoServerList.AddClick(OnGotoServerListBtnClicked);
            _btnLoginServer.AddClick(OnGameServerBtnClicked);
        }

        /// <summary>
        /// 跳转服务器列表按钮点击事件
        /// </summary>
        public void OnGotoServerListBtnClicked()
        {
            UIRoot.Instance.LoginViewCtrl.ShowWindow(WindowType.ServerListWindow);
        }

        /// <summary>
        /// 选择服务器按钮点击事件
        /// </summary>
        public void OnGameServerBtnClicked()
        {
            // TODO 服务器请求登录服务器

            SceneMgr.Instance.LoadCreateRoleScene(() =>
            {
                UIRoot.Instance.LoginViewCtrl.HideView();
                // 1、是否已有角色，是则跳转角色选择界面

                // 2、否则跳转创建角色界面
                UIRoot.Instance.CreateRoleViewCtrl.ShowWindow(WindowType.CreateRoleWindow);
            });
        }

        public void OnDestroy()
        {
            _btnGotoServerList.RemoveClick(OnGotoServerListBtnClicked);
            _btnLoginServer.RemoveClick(OnGameServerBtnClicked);
        }
    }
}
