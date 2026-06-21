using Common;
using UnityEngine;

namespace UI.Login
{
    /**
     * Title:登录视图
     * Desciption:它是登录模块所有Window的管理视图类
     **/
    public class LoginView : UIBase
    {
        [SerializeField, Header("登录窗口")] private LoginWindow _loginWindow;
        [SerializeField, Header("注册窗口")] private RegistWindow _registWindow;
        [SerializeField, Header("登录服务器窗口")] private GameServerWindow _gameServerWindow;
        [SerializeField, Header("服务器列表窗口")] private ServerListWindow _serverListWindow;

        /// <summary>
        /// 注册窗口
        /// </summary>
        protected override void RegisterWindows()
        {
            windowDict.Add(WindowType.LoginWindow, _loginWindow);
            windowDict.Add(WindowType.RegistWindow, _registWindow);
            windowDict.Add(WindowType.GameServerWindow, _gameServerWindow);
            windowDict.Add(WindowType.ServerListWindow, _serverListWindow);
        }

        /// <summary>
        /// 窗口初始化完成
        /// 默认显示登录窗口
        /// </summary>
        protected override void OnWindowsInited()
        {
            ShowWindow(WindowType.LoginWindow);
        }
    }
}
