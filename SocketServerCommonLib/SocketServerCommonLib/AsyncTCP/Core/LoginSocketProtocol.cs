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
    /// 登录时协议
    /// </summary>
    public class LoginSocketProtocol : SocketInvokeElement
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="asyncSocketServer">AsyncSocketServer</param>
        /// <param name="socketUserToken">SocketUserToken</param>
        public LoginSocketProtocol(AsyncSocketServer asyncSocketServer, SocketUserToken socketUserToken)
            : base(asyncSocketServer, socketUserToken)
        {
        }

        /// <summary>
        /// 协议通过发送客户端信息
        /// </summary>
        /// <param name="buffer">接收到的信息</param>
        /// <param name="offset">从第几位开始</param>
        /// <param name="count">共有几位</param>
        /// <returns>true</returns>
        public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
        {
            bool issuccessful = DoLogin() & DoSendResult();
            if (issuccessful)
                socketUserToken.InvokeElement = new MesgTransmitSocketProtocol(m_asyncSocketServer, socketUserToken);
            return issuccessful;
        }
        //登录
        public bool DoLogin()
        {
            socketUserToken.ActiveDateTime = DateTime.Now;
            string userName = "";
            string password = "";
            if (InDataParser.GetValue(ProtocolKeys.UserName, ref userName) & InDataParser.GetValue(ProtocolKeys.Password, ref password))
            {
                if (password.Equals(HelpCommonLib.ComminClass.MD5Encrypt("admin", 16), StringComparison.CurrentCultureIgnoreCase))
                {
                    socketUserToken.UserName = "admin";
                    if (password.Length > 4)//物联网时,区分是设备还是用户 密码大于4的是用户
                    {
                        socketUserToken.isDevice = false;
                        SocketUserSearchHorse();
                        DelegateState.ServerStateInfo(socketUserToken.ConnectSocket.RemoteEndPoint.ToString() + "用户登录成功");
                        DelegateState.AddTCPuserStateInfo(socketUserToken);
                    }
                    else
                    {
                        socketUserToken.isDevice = true;
                        m_asyncSocketServer.AsyncSocketDeviceList.Add(socketUserToken);
                        DelegateState.ServerStateInfo(socketUserToken.ConnectSocket.RemoteEndPoint.ToString() + "设备连接成功");
                        DelegateState.AddTCPdeviceStateInfo(socketUserToken);
                    }
                    socketUserToken.LoginFlag = true;
                    socketUserToken.ReceiveBuffer.Clear();
                    socketUserToken.SendBuffer.ClearPacket();

                    OutDataParser.Clear();
                    OutDataParser.AddResponse();
                    OutDataParser.AddCommand(ProtocolCodes.Success.ToString());//添加返回信息   插入-1
                    return true;
                }
                socketUserToken.LoginFlag = false;
                return false;
            }
            socketUserToken.LoginFlag = false;
            return false;
        }
        /// <summary>
        /// 发送返回值
        /// </summary>
        /// <returns>bool</returns>
        public bool DoSendResult()
        {
            try
            {
                string commandText = OutDataParser.GetProtocolText();//已经添加了返回信息          插入-1
                byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
                int totalLength = bufferUTF8.Length; //获取总大小
                AsyncSendBufferManager asyncSendBufferManager = socketUserToken.SendBuffer;
                asyncSendBufferManager.StartPacket();
                asyncSendBufferManager.m_dynamicBufferManager.WriteInt(totalLength); //写入总大小
                asyncSendBufferManager.m_dynamicBufferManager.WriteBuffer(bufferUTF8); //写入命令内容
                asyncSendBufferManager.EndPacket();

                bool result = true;
                if (!m_sendAsync)
                {
                    int packetOffset = 0;
                    int packetCount = 0;
                    if (asyncSendBufferManager.GetFirstPacket(ref packetOffset, ref packetCount))
                    {
                        m_sendAsync = true;
                        result = m_asyncSocketServer.SendAsyncEvent(socketUserToken.ConnectSocket, socketUserToken.SendEventArgs,
                            socketUserToken.ReceiveBuffer.Buffer, packetOffset, packetCount);
                    }
                }
            }
            catch { m_asyncSocketServer.CloseClientSocket(socketUserToken); }
            return true;
        }
        /// <summary>
        /// 搜索匹对设备
        /// </summary>
        public void SocketUserSearchHorse()
        {
            foreach (SocketUserToken user in m_asyncSocketServer.AsyncSocketDeviceList.Devicelist)
            {
                if (user.UserName == socketUserToken.UserName)//用户名相等
                {
                    if (user.SendUserTokenAll.Count() > 0)
                    {
                        if (user.SendUserTokenAll[0].ConnectSocket == null)
                            user.SendUserTokenAll.Clear();
                        else
                            continue;
                    }
                    socketUserToken.SendUserTokenAll.Add(user);//把用户转发端确认
                    user.SendUserTokenAll.Add(socketUserToken);//把设备回发端确认
                    break;
                }
            }
        }
    }
}
