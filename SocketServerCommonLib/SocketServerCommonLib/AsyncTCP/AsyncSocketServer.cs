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
using System.Net.Sockets;
using System.Text;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 服务器-同步监听端口、异步发送数据
    /// </summary>
    public class AsyncSocketServer
    {

        private Socket listenSocket;

        /// <summary>
        /// 是否已启动监听
        /// </summary>
        public bool IsStartListening = false;
        /// <summary>
        /// 服务器配置
        /// </summary>
        private ServerConfig Serverconfig { get; set; }

        public ServerConfig serverconfig
        {
            get { return Serverconfig; }
            set { Serverconfig = value; }
        }
        /// <summary>
        /// 用户集合
        /// </summary>
        private SocketUserClientList m_asyncSocketUserList;
        public SocketUserClientList AsyncSocketUserList { get { return m_asyncSocketUserList; } }
        /// <summary>
        /// 用户池定义
        /// </summary>
        private AsyncSocketUserPool m_asyncSocketUserTokenPool;

        /// <summary>
        /// 设备集合(通讯的话此处无需管理)
        /// </summary>
        private SocketDeviceClientList m_asyncSocketDeviceList;
        public SocketDeviceClientList AsyncSocketDeviceList
        {
            get { return m_asyncSocketDeviceList; }
            set { m_asyncSocketDeviceList = value; }
        }

        /// <summary>
        /// 用户池初始化
        /// </summary>
        public void Init()
        {
            Serverconfig = new ServerConfig();
            m_asyncSocketUserList = new SocketUserClientList();
            m_asyncSocketDeviceList = new SocketDeviceClientList();
            m_asyncSocketUserTokenPool = new AsyncSocketUserPool(serverconfig.numConnections);
            SocketUserToken userToken;
            for (int i = 0; i < serverconfig.numConnections; i++) //按照连接数建立读写对象
            {
                userToken = new SocketUserToken(serverconfig.buffSize);
                //异步回调函数初始化
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                //异步回调函数初始化
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                //开辟固定空间
                m_asyncSocketUserTokenPool.Push(userToken);
            }
        }

        /// <summary>
        /// 异步发送及接收回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            SocketUserToken userToken = asyncEventArgs.UserToken as SocketUserToken;
            userToken.ActiveDateTime = DateTime.Now;
            try
            {
                lock (userToken)
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(asyncEventArgs);
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                        ProcessSend(asyncEventArgs);
                    else
                        throw new ArgumentException("最后一次操作完成套接字接收或发送");
                }
            }
            catch (Exception ex)
            {
                DelegateState.ServerStateInfo("异步发送及接收回调函数: IO_Completed " + userToken.ConnectSocket + " error, message: " + ex.Message);
            }
        }
        /// <summary>
        /// 异步发送回调函数
        /// </summary>
        /// <param name="asyncEventArgs"></param>
        private bool ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            SocketUserToken userToken = sendEventArgs.UserToken as SocketUserToken;
            userToken.ActiveDateTime = DateTime.Now;
            if (sendEventArgs.SocketError == SocketError.Success)
                return userToken.InvokeElement.SendCompleted(userToken); //调用子类回调函数
            else
            {
                CloseClientSocket(userToken);
                return false;
            }
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="connectSocket">Socket</param>
        /// <param name="sendEventArgs">SocketAsyncEventArgs</param>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset">int</param>
        /// <param name="count">int</param>
        /// <returns>bool</returns>
        public bool SendAsyncEvent(Socket connectSocket, SocketAsyncEventArgs sendEventArgs, byte[] buffer, int offset, int count)
        {
            if (connectSocket == null)
                return false;
            sendEventArgs.SetBuffer(buffer, offset, count);//设置缓冲区域
            bool willRaiseEvent = connectSocket.SendAsync(sendEventArgs);//异步发送
            if (!willRaiseEvent)
            {
                return ProcessSend(sendEventArgs);
            }
            else
                return true;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void Start()
        {
            if (IsStartListening)
                return;

            IsStartListening = true;
            Init();
            listenSocket = new Socket(serverconfig.locahostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(serverconfig.locahostEndPoint);
            listenSocket.Listen(serverconfig.numConnections);
            DelegateState.ServerStateInfo("TCP服务器启动...");
            StartAccept(null);
            serverconfig.m_daemonThread = new DaemonThread(this);//启动连接超时判断
        }

        /// <summary>
        /// 监听程序
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)//第一运
            {
                //acceptEventArgs == null  绑定异步回调函数AcceptEventArg_Completed
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                //为什么没有while ?  AcceptEventArg_Completed()异步回调函数中递归了StartAccept()方法
                //再次调用StartAccept()方法时候AcceptSocket处于上一个用户的绑定状态,所以释放上次绑定的Socket，等待下一个Socket连接
                acceptEventArgs.AcceptSocket = null;
            }

            serverconfig.semap.WaitOne();//减少个一信号量 ，退出时候会返回
            //异步监控：回调函数AcceptEventArg_Completed
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArgs);
            if (!willRaiseEvent)
            {
                //不成功再次连接
                ProcessAccept(acceptEventArgs);
            }
        }
        /// <summary>
        /// 异步监控回调函数
        /// </summary>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                ProcessAccept(e);
            }
            catch (Exception ex)
            {
                DelegateState.ServerStateInfo("连接异常:" + ex.Message);
                //DelegateState.ServerStateInfo(ex.StackTrace);
            }
        }
        /// <summary>
        /// 添加新客户
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs.AcceptSocket.RemoteEndPoint == null)
                throw new Exception("服务器停止.");

            DelegateState.ServerStateInfo(" TCP - 客户端：" + acceptEventArgs.AcceptSocket.RemoteEndPoint + "连接");
            DelegateState.ServerConnStateInfo(acceptEventArgs.AcceptSocket.RemoteEndPoint.ToString(), "TCP");

            SocketUserToken userToken = m_asyncSocketUserTokenPool.Pop();
            m_asyncSocketUserList.Add(userToken);
            userToken.ConnectSocket = acceptEventArgs.AcceptSocket;
            userToken.ConnectDateTime = DateTime.Now;
            try
            {
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs);//异步回调函数确定
                if (!willRaiseEvent)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            catch (Exception e)
            {
                DelegateState.ServerStateInfo("连接端 " + userToken.ConnectSocket + " 错误, 错误信息: " + e.Message);
            }
            StartAccept(acceptEventArgs);//递归继续异步监控客户端
        }

        /// <summary>
        /// 异步接收请求
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void ProcessReceive(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            SocketUserToken userToken = socketAsyncEventArgs.UserToken as SocketUserToken;
            if (userToken.ConnectSocket == null)
                return;

            userToken.ActiveDateTime = DateTime.Now;

            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                int offset = userToken.ReceiveEventArgs.Offset;
                int count = userToken.ReceiveEventArgs.BytesTransferred;
                if ((userToken.InvokeElement == null) & (userToken.ConnectSocket != null)) // 第一次发送的数据为-初始话是登录模式 还是 信息模式
                {
                    BuildingInvokeElement(userToken);
                    offset = offset + 1;
                    count = count - 1;
                }

                if (userToken.InvokeElement == null) //如果没有解析对象，提示非法连接并关闭连接
                {
                    DelegateState.ServerStateInfo("非法连接:" + userToken.ConnectSocket.RemoteEndPoint);
                    CloseClientSocket(userToken);
                }
                else
                {
                    if (count > 0) //处理接收数据
                    {
                        if (!userToken.InvokeElement.ProcessReceive(userToken.ReceiveEventArgs.Buffer, offset, count))//处理数据
                        {
                            //如果处理数据返回失败，则断开连接
                            CloseClientSocket(userToken);
                        }
                        else
                        {
                            bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //继续异步接收
                            if (!willRaiseEvent)
                                ProcessReceive(userToken.ReceiveEventArgs);
                        }
                    }
                }

            }
            else
            {
                DelegateState.ServerStateInfo(string.Format("空数据,断线 {0}",
userToken.ConnectSocket.RemoteEndPoint));
                CloseClientSocket(userToken);
            }
        }
        /// <summary>
        /// 初始化类,确定是登录或者发送信息 和其它的API
        /// </summary>
        /// <param name="userToken"></param>
        private void BuildingInvokeElement(SocketUserToken userToken)
        {
            //获取接收的0个字节-初始化是登录还是信息
            byte flag = userToken.ReceiveEventArgs.Buffer[userToken.ReceiveEventArgs.Offset];
            if (flag == (byte)ProtocolFlags.Login)
                userToken.InvokeElement = new LoginSocketProtocol(this, userToken);

            if (userToken.InvokeElement != null)
            {
                DelegateState.ServerStateInfo(userToken.ConnectSocket.RemoteEndPoint + "登录初始化");
            }
        }

        /// <summary>
        /// 清除客户端
        /// </summary>
        /// <param name="userToken"></param>
        public void CloseClientSocket(SocketUserToken userToken)
        {
            if (userToken.ConnectSocket == null)
                return;

            DelegateState.ReomveTCPStateInfo(userToken);
            string socketInfo = string.Format(" 删除地址: {0}",
                userToken.ConnectSocket.RemoteEndPoint);
            try
            {
                userToken.SendUserTokenAll.Clear();//清除设备连接
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception E)
            {
                DelegateState.ServerStateInfo("断开连接 " + socketInfo + " error, message: {1}" + E.Message);
            }
            userToken.LoginFlag = false;
            userToken.ConnectSocket.Close();
            userToken.ConnectSocket = null; //释放引用，并清理缓存，包括释放协议对象等资源
            serverconfig.semap.Release();//增加个一信号量
            m_asyncSocketUserTokenPool.Push(userToken);
            AsyncSocketUserList.Remove(userToken);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="commandText">string</param>
        /// <param name="userToken">SocketUserToken 连接客户端</param>
        /// <returns>bool</returns>
        public bool SendMsgClientMsg(string commandText, SocketUserToken userToken)
        {
            byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
            int totalLength = bufferUTF8.Length; //获取总大小
            AsyncSendBufferManager asyncSendBufferManager = userToken.SendBuffer;
            asyncSendBufferManager.StartPacket();
            asyncSendBufferManager.m_dynamicBufferManager.WriteBuffer(bufferUTF8); //写入命令内容
            asyncSendBufferManager.EndPacket();
            bool result = true;
            if (!userToken.m_sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (asyncSendBufferManager.GetFirstPacket(ref packetOffset, ref packetCount))
                {
                    userToken.m_sendAsync = true;
                    result = SendAsyncEvent(userToken.ConnectSocket, userToken.SendEventArgs,
                        asyncSendBufferManager.m_dynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            Serverconfig.m_daemonThread.Close();
            IsStartListening = false;
            listenSocket.Close();
            m_asyncSocketUserList.Userlist = null;
            m_asyncSocketDeviceList = null;
            GC.Collect();
        }
    }
}
