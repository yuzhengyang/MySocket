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
using System.Net.Sockets;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class SocketUserToken
    {
        /// <summary>
        ///  接收缓冲区域
        /// </summary>
        protected DynamicBufferManager m_receiveBuffer;
        public DynamicBufferManager ReceiveBuffer { get { return m_receiveBuffer; } set { m_receiveBuffer = value; } }

        /// <summary>
        ///发送缓冲区域
        /// </summary>
        protected AsyncSendBufferManager m_sendBuffer;
        public AsyncSendBufferManager SendBuffer { get { return m_sendBuffer; } set { m_sendBuffer = value; } }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int BuffSize = 1024 * 10;
        /// <summary>
        /// 接收套节字
        /// </summary>
        public NetworkStream m_nStream { get; set; }

        /// <summary>
        /// 异步接收后包的大小
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 当前连接服务端端口号
        /// </summary>
        public string m_ServerIp;
        public int m_ServerPort;

        /// <summary>
        /// 搜索设备的对应名称
        /// </summary>
        public string UserName;
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string OnlyName;

        /// <summary>
        /// 客户端
        /// </summary>
        TcpClient m_client;

        /// <summary>
        /// 创建Sockets对象
        /// </summary>
        /// <param name="ip">Ip地址</param>
        /// <param name="client">TcpClient</param>
        /// <param name="ns">承载客户端Socket的网络流</param>
        public SocketUserToken(string ServerIp, int ServerPort, TcpClient client, NetworkStream nStream)
        {
            m_ServerIp = ServerIp;
            m_ServerPort = ServerPort;
            m_client = client;
            m_nStream = nStream;
            m_receiveBuffer = new DynamicBufferManager(BuffSize);
            m_sendBuffer = new AsyncSendBufferManager(BuffSize);
        }
    }
}
