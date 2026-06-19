using UnityEngine;
using UI.Login;

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

    // 显示登录视图
    public override void ShowView()
    {
        _loginView.Show();
    }

    // 隐藏登录视图
    public override void HideView()
    {
        _loginView.Hide();
    }

    /// <summary>
    /// 根据WindowType显示对应的窗口
    /// </summary>
    /// <param name="windowType">窗口类型</param>
    public void ShowWindow(WindowType windowType)
    {
        _loginView.ShowWindow(windowType);
    }
}
