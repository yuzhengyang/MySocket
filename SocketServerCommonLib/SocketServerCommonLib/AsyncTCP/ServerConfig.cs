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
using System.Net;
using System.Threading;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 服务器配置
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// 最大支持数量
        /// </summary>
        public int numConnections = 50000;

        /// <summary>
        /// 限制访问接收连接的线程数，用来控制最大并发数-如果有numConnections 线程全部阻塞,等待一个用户退出,才能继续
        /// </summary>
        public Semaphore semap = new Semaphore(50000, 50000);

        /// <summary>
        /// 监听IP-端口
        /// </summary>
        public string ListenIp = "127.0.0.1";
        public int ListenPort = 16000;
        public IPEndPoint locahostEndPoint;
        /// <summary>
        /// Socket连接超时(毫秒),检测Socket是否在线间隔
        /// </summary>
        public int socketTimeoutMs = 6000;

        /// <summary>
        /// 守护连接进程
        /// </summary>
        public DaemonThread m_daemonThread;

        /// <summary>
        /// 缓存大小
        /// </summary>
        public int buffSize = 1024 * 1;

        public ServerConfig()
        {
            ListenIp = HelpCommonLib.NetworkAddress.GetIPAddress();
            locahostEndPoint = new IPEndPoint(IPAddress.Parse(ListenIp), ListenPort);
        }
    }
}
