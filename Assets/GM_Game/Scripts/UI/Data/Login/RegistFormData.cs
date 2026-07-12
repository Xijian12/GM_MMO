namespace UI.Login.Data
{
    /**
     * Title:注册表单数据
     * Desciption:RegistWindow 校验通过后提交给 LoginCtrl，由 Ctrl 组装 RegistReq。
     **/
    public readonly struct RegistFormData
    {
        public readonly string UserName;
        public readonly string PhoneNum;
        public readonly string Password;
        public readonly string VerifyCode;

        public RegistFormData(string userName, string phoneNum, string password, string verifyCode)
        {
            UserName = userName;
            PhoneNum = phoneNum;
            Password = password;
            VerifyCode = verifyCode;
        }
    }
}
