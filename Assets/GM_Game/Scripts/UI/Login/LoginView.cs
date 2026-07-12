using Common;
using System;
using UI.Login.Data;
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

        /// <summary>
        /// 注册登录游戏服务器事件
        /// </summary>
        /// <param name="action"></param>
        public void RegisterGameServerBtnClicked(Action<GameServer> action)
        {
            _gameServerWindow.GameServerBtnClickAction += action;
        }

        /// <summary>
        /// 取消注册登录游戏服务器事件
        /// </summary>
        /// <param name="action"></param>
        public void UnRegisterGameServerBtnClicked(Action<GameServer> action)
        {
            _gameServerWindow.GameServerBtnClickAction -= action;
        }

        /// <summary>
        /// 注册请求游戏服务器列表事件
        /// </summary>
        /// <param name="action"></param>
        public void RegisterGotoServerListBtnClicked(Action action)
        {
            _gameServerWindow.GotoServerListBtnClickAction += action;
        }

        /// <summary>
        /// 取消请求游戏服务器列表事件
        /// </summary>
        /// <param name="action"></param>
        public void UnRegisterGotoServerListBtnClicked(Action action)
        {
            _gameServerWindow.GotoServerListBtnClickAction -= action;
        }

        /// <summary>
        /// 注册登录提交事件
        /// </summary>
        public void RegisterLoginSubmit(Action<LoginFormData> action)
        {
            _loginWindow.OnLoginSubmit += action;
        }

        /// <summary>
        /// 取消注册登录提交事件
        /// </summary>
        public void UnregisterLoginSubmit(Action<LoginFormData> action)
        {
            _loginWindow.OnLoginSubmit -= action;
        }

        /// <summary>
        /// 注册提交事件
        /// </summary>
        public void RegisterRegistSubmit(Action<RegistFormData> action)
        {
            _registWindow.OnRegistSubmit += action;
        }

        /// <summary>
        /// 取消注册提交事件
        /// </summary>
        public void UnregisterRegistSubmit(Action<RegistFormData> action)
        {
            _registWindow.OnRegistSubmit -= action;
        }
    }
}
