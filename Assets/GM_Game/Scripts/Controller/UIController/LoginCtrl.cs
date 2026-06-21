using Common;
using UI;
using UnityEngine;

namespace UI.Login
{
    /**
     * Title:登录控制器
     * Desciption:对登录视图进行管理（显示、隐藏、数据操作）
     **/
    public class LoginCtrl : CtrlBase
    {
        private readonly LoginView _loginView;

        public LoginCtrl(UIBase view) : base(view)
        {
            _loginView = view as LoginView;
            _loginView.InitView();
        }
    }
}
