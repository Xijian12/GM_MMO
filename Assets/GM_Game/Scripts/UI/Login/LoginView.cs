using System.Collections.Generic;
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

        private Dictionary<WindowType, UIBase> _windowDict;

        public override void InitView()
        {
            _windowDict = new Dictionary<WindowType, UIBase>
            {
                { WindowType.LoginWindow, _loginWindow },
                { WindowType.RegistWindow, _registWindow }
            };

            foreach (var window in _windowDict.Values)
            {
                window.InitView();
            }

            ShowWindow(WindowType.LoginWindow);
        }

        /// <summary>
        /// 根据WindowType返回对应的窗口
        /// </summary>
        /// <param name="windowType">窗口类型</param>
        /// <returns>对应的窗口</returns>
        public UIBase GetWindow(WindowType windowType)
        {
            return _windowDict[windowType];
        }

        /// <summary>
        /// 根据WindowType显示对应的窗口
        /// </summary>
        /// <param name="windowType">窗口类型</param>
        public void ShowWindow(WindowType windowType)
        {
            // 先隐藏所有窗口
            foreach (var window in _windowDict.Values)
            {
                window.Hide();
            }
            // 显示对应的窗口
            _windowDict[windowType].Show();
        }
    }
}