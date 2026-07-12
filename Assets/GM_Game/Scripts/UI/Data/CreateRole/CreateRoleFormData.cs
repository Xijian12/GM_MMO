namespace UI.CreateRole.Data
{
    /**
     * Title:创建角色表单数据
     * Desciption:CreateRoleWindow 校验通过后提交给 CreateRoleCtrl，由 Ctrl 组装 CreateRoleReq。
     **/
    public readonly struct CreateRoleFormData
    {
        public readonly string Nickname;
        public readonly int JobId;

        public CreateRoleFormData(string nickname, int jobId)
        {
            Nickname = nickname;
            JobId = jobId;
        }
    }
}
