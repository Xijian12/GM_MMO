using UnityEngine;

/**
 * Title:登录视图
 * Desciption:它是登录模块所有Window的管理视图类
 **/
public class LoginView : UIBase
{
    [SerializeField, Header("登录窗口")] private LoginWindow _loginWindow;

    public override void InitView()
    {

    }

    public UIBase GetWindow(WindowType windowType)
    {
        switch (windowType)
        {
            case WindowType.LoginWindow:
                return _loginWindow;
            default:
                return _loginWindow;
        }

    }
}
