namespace UI.Login.Data
{
    /**
     * Title:登录表单数据
     * Desciption:LoginWindow 校验通过后提交给 LoginCtrl，由 Ctrl 组装 LoginReq。
     **/
    public readonly struct LoginFormData
    {
        public readonly string UserName;
        public readonly string Password;

        public LoginFormData(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
