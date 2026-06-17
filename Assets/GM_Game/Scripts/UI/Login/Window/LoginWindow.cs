using TMPro;
using UnityEngine;
using UnityEngine.UI;


/**
 * Title:登录窗口
 * Desciption:
 **/
public class LoginWindow : UIBase
{
    [SerializeField, Header("账号输入框")] private TMP_InputField _iptAcct;
    [SerializeField, Header("密码输入框")] private TMP_InputField _iptPasd;
    [SerializeField, Header("记住账号Toggle")] private Toggle _toRemeberAcct;
    [SerializeField, Header("用户协议Toggle")] private Toggle _toAgreement;


}
