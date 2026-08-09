namespace Common
{
    /// <summary>
    /// 窗口类型
    /// </summary>
    public enum WindowType
    {
        LoginWindow,    // 登录窗口
        RegistWindow,   // 注册窗口
        GameServerWindow, // 游戏服务器窗口
        ServerListWindow, // 服务器列表窗口
        CreateRoleWindow, // 创建角色窗口
        SelectRoleWindow, // 选择角色窗口
    }

    public enum RoleState
    {
        // 调整角色默认状态为Idle，否则不能直接进行跳跃
        Idle,
        Run,
        FastRun,
        Jump,
        Slider
    }
}
