using Common;
using TMPro;
using UI;
using UnityEngine;

namespace UI.CreateRole
{
    /**
     * Title:创建角色窗口
     * Desciption:目前只有一个角色，所以创建的角色职业默认时剑修
     **/
    public class CreateRoleWindow : WindowBase
    {
        [SerializeField, Header("角色昵称输入框")] private TMP_InputField _iptNickName;

        [SerializeField, Header("创建角色按钮")] private UGUIBtn _btnCreateRole;

        public override void InitView()
        {
            base.InitView();
            _btnCreateRole.AddClick(OnCreateRoleBtnClick);
        }

        public void OnCreateRoleBtnClick()
        {
            // 判断输入框是否为空
            // 验证昵称是否合法
            // 服务器验证，是否创建成功
            // 跳转选择角色界面
            UIRoot.Instance.CreateRoleViewCtrl.ShowWindow(WindowType.SelectRoleWindow);
        }

        public void OnDestroy()
        {
            _btnCreateRole.RemoveClick(OnCreateRoleBtnClick);
        }
    }
}
