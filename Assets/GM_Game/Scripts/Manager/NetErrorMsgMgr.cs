using Common;
using Google.Protobuf;
using System;
using UnityEngine;

namespace Manager
{
    /**
 	* Title:网络错误消息管理类
 	* Desciption:
 	**/
    public class NetErrorMsgMgr : Singleton<NetErrorMsgMgr>
    {
        public void Init()
        {
            // 注册错误相关消息
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_ErrCode, OnErrorMsgHandle);
        }

        /// <summary>
        /// 处理错误消息
        /// </summary>
        /// <param name="data"></param>
        private void OnErrorMsgHandle(ByteString data)
        {
            ErrMsg errMsg = ErrMsg.Parser.ParseFrom(data);

            switch (errMsg.CmdCode)
            {
                case CmdCode.AcctExist:
                    TipsMgr.Instance.ShowSystemTips("账号已存在...");
                    break;
                case CmdCode.ServerError:
                    TipsMgr.Instance.ShowSystemTips("服务端错误...");
                    break;
                case CmdCode.AcctNotExist:
                    TipsMgr.Instance.ShowSystemTips("账号不存在...");
                    break;
                case CmdCode.PasswordError:
                    TipsMgr.Instance.ShowSystemTips("密码...");
                    break;
                case CmdCode.AcctDisable:
                    TipsMgr.Instance.ShowSystemTips("账号已禁用...");
                    break;
                case CmdCode.ReqParamError:
                    TipsMgr.Instance.ShowSystemTips("角色昵称重复...");
                    break;
                case CmdCode.NicknameExist:
                    TipsMgr.Instance.ShowSystemTips("昵称已存在...");
                    break;
                case CmdCode.RoleExist:
                    TipsMgr.Instance.ShowSystemTips("角色已存在...");
                    break;
                case CmdCode.UserNameIllegal:
                    TipsMgr.Instance.ShowSystemTips("用户名不合法...");
                    break;
                case CmdCode.PhoneNumIllegal:
                    TipsMgr.Instance.ShowSystemTips("手机号码不合法...");
                    break;
                case CmdCode.PasswordIllegal:
                    TipsMgr.Instance.ShowSystemTips("密码不合法...");
                    break;
                case CmdCode.NicknameIllegal:
                    TipsMgr.Instance.ShowSystemTips("用户名不合法...");
                    break;
                case CmdCode.UserOftenLogin:
                    TipsMgr.Instance.ShowSystemTips("用户频繁登录，请稍等...");
                    break;
                default:
                    break;
            }
        }
    }
}
