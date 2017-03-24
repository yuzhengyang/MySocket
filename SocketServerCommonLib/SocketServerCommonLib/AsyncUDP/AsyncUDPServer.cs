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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServerCommonLib
{
    /// <summary>
    /// Socket UDP-UDP远程连接P2P,不做转发,不做大量数据接收(只做NAT穿透服务器)(未做用户,设备集合未及时清理)(可以直接发送2直接获取设备)
    /// </summary>
    public class AsyncUDPServer
    {
        /// <summary>
        /// Socket字节套
        /// </summary>
        public Socket m_sListen;
        /// <summary>
        /// 是否已启动监听
        /// </summary>
        public bool IsStartListening = false;
        /// <summary>
        /// 心跳检测(毫秒)
        /// </summary>
        public int socketTimeoutMs { get; set; }
        /// <summary>
        /// 客户端集合
        /// </summary>
        private List<SocketUserUDP> userInfoList;
        /// <summary>
        /// 设备端集合
        /// </summary>
        public List<SocketUserUDP> DeviceInfoList{get;set;}
        /// <summary>
        /// 服务器绑定地址
        /// </summary>
        private IPEndPoint loclhostIpEndPoint;
        /// <summary>
        /// 客户端
        /// </summary>
        private EndPoint RemoteEndPoint;
        /// <summary>
        /// 缓存地址
        /// </summary>
        public byte[] BufferData;
        /// <summary>
        /// 监听端口
        /// </summary>
        public int ListenProt = 5000;
        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore semap;
        /// <summary>
        /// 线程监听
        /// </summary>
        Thread thread;
        /// <summary>
        /// 心跳线程
        /// </summary>
        DaemonThreadUDP m_DaemonThread;

        public List<SocketUserUDP> UserInfoList
        {
            get { return userInfoList; }
            set { userInfoList = value; }
        }

        public AsyncUDPServer()
        {
            socketTimeoutMs = 6000;
            BufferData = new byte[1024 * 20];
            loclhostIpEndPoint = new IPEndPoint(IPAddress.Any, ListenProt);
            UserInfoList = new List<SocketUserUDP>();
            DeviceInfoList = new List<SocketUserUDP>();
            m_sListen = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_sListen.Bind(loclhostIpEndPoint);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            RemoteEndPoint = (EndPoint)(sender);
        }

        /// <summary>
        /// 启动UDP
        /// </summary>
        public void Start()
        {
            if (IsStartListening)
                return;

            semap = new Semaphore(30000, 30000);
            IsStartListening = true;
            thread = new Thread(new ThreadStart(StartAccept));
            thread.Start();
            m_DaemonThread = new DaemonThreadUDP(this);
            DelegateState.ServerStateInfo("UDP服务器启动...");
        }

        /// <summary>
        /// 监控
        /// </summary>
        public void StartAccept()
        {
            //异步操作
            //m_sListen.BeginReceiveFrom(
            //m_sListen.Buffer, 0, state.Buffer.Length,
            //m_sListen.None,
            //ref m_sListen.RemoteEP,
            //EndReceiveFromCallback,
            //state);
            m_sListen.ReceiveFrom(BufferData, ref RemoteEndPoint);
            semap.WaitOne();
            if (BufferData[0] == 0x1)
            {
                string username = Encoding.UTF8.GetString(BufferData, 1, BufferData.Length);
                SocketUserUDP userUdp = new SocketUserUDP();
                userUdp.ipEndPoint = RemoteEndPoint;
                userUdp.ActiveDateTime = DateTime.Now;
                userUdp.UserName = username;
                userUdp.password = username;
                if (userUdp.password.Length > 4)
                {
                    //密码小于4是设备
                    DeviceInfoList.Add(userUdp);
                    DelegateState.ServerStateInfo(RemoteEndPoint.ToString() + "远端设备连接");
                }
                else
                {
                    UserInfoList.Add(userUdp);
                    DelegateState.ServerStateInfo(RemoteEndPoint.ToString() + "远端用户连接");
                }
                m_sListen.SendTo(Encoding.UTF8.GetBytes("连接成功!"), RemoteEndPoint);
                DelegateState.ServerConnStateInfo(RemoteEndPoint.ToString(), "UDP");
            }
            else if (BufferData[0] == 0x2)
            {
                string username = Encoding.UTF8.GetString(BufferData, 1, BufferData.Length);
                foreach (SocketUserUDP user in DeviceInfoList)
                {
                    if (user.UserName == username)
                    {
                        user.ActiveDateTime = DateTime.Now;
                        m_sListen.SendTo(Encoding.UTF8.GetBytes(user.ipEndPoint.ToString()), RemoteEndPoint);
                        DelegateState.ServerStateInfo(RemoteEndPoint.ToString() + "远端用户:" + user.UserName + "搜索设备.");
                    }
                }
            }
            else if (BufferData[0] == 0x3)
            {
                string username = Encoding.UTF8.GetString(BufferData, 1, BufferData.Length);
                foreach (SocketUserUDP user in UserInfoList)
                {
                    if (user.UserName == username)
                    {
                        DeviceInfoList.Remove(user);
                        DelegateState.ServerStateInfo("UDP:" + RemoteEndPoint.ToString() + "远端用户退出");
                        break;
                    }
                }
            }
            else
            {
                DelegateState.ServerStateInfo("UDP:" + RemoteEndPoint.ToString() + "发送空数据");
            }
            semap.Release();
            StartAccept();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            thread.Abort();
            m_sListen.Shutdown(SocketShutdown.Both);
            DeviceInfoList.Clear();
            userInfoList.Clear();
            m_sListen.Close();
            IsStartListening = false;
            GC.Collect();
        }
    }
}
