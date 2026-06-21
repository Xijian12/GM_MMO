using Common;
using UI;
using UnityEngine;

namespace UI.CreateRole
{
    /**
     * Title:创建角色和选择角色
     * Desciption:
     **/
    public class CreateRoleView : UIBase
    {
        [SerializeField, Header("创建角色窗口")] private CreateRoleWindow _createRoleWindow;
        [SerializeField, Header("选择角色窗口")] private SelectRoleWindow _selectRoleWindow;

        /// <summary>
        /// 注册窗口
        /// </summary>
        protected override void RegisterWindows()
        {
            windowDict.Add(WindowType.CreateRoleWindow, _createRoleWindow);
            windowDict.Add(WindowType.SelectRoleWindow, _selectRoleWindow);
        }

        /// <summary>
        /// 窗口初始化完成
        /// </summary>
        protected override void OnWindowsInited()
        {
            ShowWindow(WindowType.CreateRoleWindow);
        }
    }
}
