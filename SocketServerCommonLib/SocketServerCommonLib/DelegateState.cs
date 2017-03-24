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

namespace SocketServerCommonLib
{
    /// <summary>
    /// 委托回调函数 this.Invoke(new ThreadStart(delegate{})) 实现与UI交换
    /// </summary>
    public class DelegateState
    {
        //信息显示
        public delegate void SocketStateCallBack(string msg);

        public delegate void SockeTeartbeatStateCallBack(int num);
        public delegate void SocketConnStateCallBack(string RemoteIp,string TCPUDP);
        /// <summary>
        /// 信息显示
        /// </summary>
        public static SocketStateCallBack ServerStateInfo;

        /// <summary>
        /// 心跳检测信息
        /// </summary>
        public static SockeTeartbeatStateCallBack TeartbeatServerStateInfo;

        /// <summary>
        /// 信息显示
        /// </summary>
        public static SocketConnStateCallBack ServerConnStateInfo;


        #region TCP服务
        
        public delegate void SocketTCPStateCallBack(string msg);
        public delegate void SocketAddTCPuserStateCallBack(SocketUserToken userToken);

        public delegate void SocketAddTCPdeviceStateCallBack(SocketUserToken userToken);
        public delegate void SocketReomveTCPStateCallBack(SocketUserToken userToken);

        /// <summary>
        /// TCP信息显示
        /// </summary>
        public static SocketTCPStateCallBack ServerTCPStateInfo;
        /// <summary>
        /// TCP添加用户
        /// </summary>
        public static SocketAddTCPuserStateCallBack AddTCPuserStateInfo;
        /// <summary>
        /// TCP添加设备
        /// </summary>
        public static SocketAddTCPdeviceStateCallBack AddTCPdeviceStateInfo;
        /// <summary>
        /// TCP删除连接
        /// </summary>
        public static SocketReomveTCPStateCallBack ReomveTCPStateInfo;
        #endregion
        
        #region UDP服务

        public delegate void SocketUDPStateCallBack(string msg);
        public delegate void SocketAddUDPStateCallBack(SocketUserUDP userToken);
        public delegate void SocketReomveUDPStateCallBack(SocketUserToken userToken);

        /// <summary>
        /// UDP信息显示
        /// </summary>
        public static SocketUDPStateCallBack ServerUDPStateInfo;
        /// <summary>
        /// UDP添加用户
        /// </summary>
        public static SocketAddUDPStateCallBack AddUDPStateInfo;
        /// <summary>
        /// UDP删除用户
        /// </summary>
        public static SocketReomveUDPStateCallBack ReomveUDPStateInfo;
        #endregion

    }
}
