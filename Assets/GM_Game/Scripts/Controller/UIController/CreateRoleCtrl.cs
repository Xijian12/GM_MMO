using Common;
using UI;
using UnityEngine;

namespace UI.CreateRole
{
    /**
     * Title:创建角色控制器
     * Desciption:
     **/
    public class CreateRoleCtrl : CtrlBase
    {
        private readonly CreateRoleView _createRoleView;

        public CreateRoleCtrl(UIBase view) : base(view)
        {
            _createRoleView = view as CreateRoleView;
            _createRoleView.InitView();
        }
    }
}
