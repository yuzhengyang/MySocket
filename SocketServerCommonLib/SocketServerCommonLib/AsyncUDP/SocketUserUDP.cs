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
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServerCommonLib
{
    /// <summary>
    /// UDP 用户
    /// </summary>
    public class SocketUserUDP
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码 根据长度判断是否是设备
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 登录唯一标识
        /// </summary>
        public string OnlyLogin { get; set; }
        /// <summary>
        /// 消息时间 心跳时判断
        /// </summary>
        public DateTime ActiveDateTime;
        /// <summary>
        /// 连接客户端
        /// </summary>
        public Socket m_connectSocket;
        /// <summary>
        /// 客户端地址
        /// </summary>
        public EndPoint ipEndPoint { get; set; }
        /// <summary>
        /// 接收缓存区
        /// </summary>
        public byte[] RecBuffer = new byte[1204];
    }
}
