/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 协议初始定义
    /// </summary>
    class ProtocolKeys
    {
        //判断是否存在
        public static string ReturnWrap = "\r\n";
        //值
        public static string Command = "Command";
        //用户名
        public static string UserName = "UserName";
        //密码
        public static string Password = "Password";
        //唯一标志
        public static string OnlyUser = "Only";
        //Command= ?    UserName= ? 定义值
        public static string EqualSign = "=";

        //状态 ProtocolCode
        public static string Code = "Code";


        public static string LeftBrackets = "[";
        public static string RightBrackets = "]";
        //返回
        public static string Request = "Request";
        //发送
        public static string Response = "Response";

        //密钥
        public static string PubKey = "PubKey";
    }

    /// <summary>
    /// 状态码
    /// </summary>
    public class ProtocolCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static int Success = 0x00000000;
        /// <summary>
        /// 失败
        /// </summary>
        public static int failure = 0x10000000;
    }
    //标记信息
    public enum ProtocolFlags
    {
        /// <summary>
        /// 登录
        /// </summary>
        Login = 4, 
        /// <summary>
        /// 信息
        /// </summary>
        Information = 5,
    }
}
