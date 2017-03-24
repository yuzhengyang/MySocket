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
using System.Threading;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 客户端使用类
    /// </summary>
    public class SyncSocketClient
    {
        protected TcpClient m_tcpClient;
        /// <summary>
        /// 客户端配置
        /// </summary>
        protected SocketUserToken UserToken;


        /// <summary>
        /// 发送信息时组装信息协议(信息组装后发送客户端)
        /// </summary>
        private AssemblyOutDataParser m_outDataParser;
        public AssemblyOutDataParser OutDataParser { get { return m_outDataParser; } set { m_outDataParser = value; } }

        /// <summary>
        /// 接收时候信息组装信息(确定信息是否合格)
        /// </summary>
        private AssemblyInDataParser m_InDataParser;
        public AssemblyInDataParser InDataParser { get { return m_InDataParser; } set { m_InDataParser = value; } }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">IP</param>
        /// <param name="port">地址</param>
        public void Connect(string Serverip, int Port)
        {
            m_outDataParser = new AssemblyOutDataParser();
            m_InDataParser = new AssemblyInDataParser();

            m_tcpClient = new TcpClient();
            m_tcpClient.Client.Blocking = true;
            m_tcpClient.Client.Bind(new IPEndPoint(IPAddress.Parse(Serverip), Port));
            m_tcpClient.Connect(Serverip, Port);
            m_tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            NetworkStream nStream = new NetworkStream(m_tcpClient.Client, true);
            UserToken = new SocketUserToken(Serverip, Port, m_tcpClient, nStream);
            UserToken.m_nStream.BeginRead(UserToken.ReceiveBuffer.Buffer, 0, UserToken.ReceiveBuffer.Buffer.Length, new AsyncCallback(EndReader), UserToken);
        }

        /// <summary>
        /// 异步接收回调函数
        /// </summary>
        /// <param name="ir"></param>
        private void EndReader(IAsyncResult ir)
        {
            SocketUserToken ir_UserToken = ir.AsyncState as SocketUserToken;
            try
            {
                if (ir_UserToken.m_nStream != null)
                {
                    if (m_tcpClient == null)
                    {
                        ir_UserToken.m_nStream.Close();
                        ir_UserToken.m_nStream.Dispose();
                        return;
                    }
                    ir_UserToken.Offset = ir_UserToken.m_nStream.EndRead(ir);
                    byte[] buffer = new byte[ir_UserToken.Offset];
                    Array.Copy(ir_UserToken.ReceiveBuffer.Buffer, buffer, ir_UserToken.Offset);
                    string tmpStr = Encoding.UTF8.GetString(buffer, 0, ir_UserToken.Offset);
                    if (!m_InDataParser.DecodeProtocolText(tmpStr))//组装成功则进入
                    {
                        ir_UserToken.ReceiveBuffer.Clear(ir_UserToken.Offset);//接收完成后清除掉前面的缓存
                        //组装接收的协议
                        DelegateState.ServerStateInfo("信息" + Encoding.UTF8.GetString(buffer));//处理信息
                    }
                    UserToken.m_nStream.BeginRead(UserToken.ReceiveBuffer.Buffer, 0, UserToken.ReceiveBuffer.Buffer.Length, new AsyncCallback(EndReader), UserToken);
                }
            }
            catch (Exception ex)
            {
                DelegateState.ServerStateInfo("接收异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Disconnect()
        {
            m_tcpClient.Client.Shutdown(SocketShutdown.Both);
            m_tcpClient.Close();
            m_tcpClient = null;
        }

        /// <summary>
        /// 组包发送
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public bool SendCommand()
        {
            //OutDataParser.GetProtocolText()结合使用 下
            OutDataParser.Clear();
            OutDataParser.AddResponse();
            OutDataParser.AddValue(ProtocolKeys.UserName, "admin");
            OutDataParser.AddValue(ProtocolKeys.Password, HelpCommonLib.ComminClass.MD5Encrypt("admin", 16));

            string commandText = OutDataParser.GetProtocolText();
            byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
            int totalLength = bufferUTF8.Length; //获取总大小
            UserToken.SendBuffer.StartPacket();
            UserToken.SendBuffer.m_dynamicBufferManager.WriteInt(totalLength); //写入总大小
            UserToken.SendBuffer.m_dynamicBufferManager.WriteBuffer(bufferUTF8); //写入命令内容
            UserToken.SendBuffer.EndPacket();

            int packetOffset = 0;
            int packetCount = 0;
            if (UserToken.SendBuffer.GetFirstPacket(ref packetOffset, ref packetCount))
            {
                m_tcpClient.Client.Send(UserToken.SendBuffer.m_dynamicBufferManager.Buffer, 0, UserToken.SendBuffer.m_dynamicBufferManager.DataCount, SocketFlags.None); //使用阻塞模式，Socket会一次发送完所有数据后才返回
                while (UserToken.SendBuffer.GetFirstPacket(ref packetOffset, ref packetCount))
                {
                    UserToken.SendBuffer.ClearFirstPacket();
                    m_tcpClient.Client.Send(UserToken.SendBuffer.m_dynamicBufferManager.Buffer, 0, UserToken.SendBuffer.m_dynamicBufferManager.DataCount, SocketFlags.None); //使用阻塞模式，Socket会一次发送完所有数据后才返回    
                }
            }
            return true;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="commandText">信息</param>
        /// <returns>bool</returns>
        public bool SendCommand(string commandText)
        {

            byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
            int totalLength = sizeof(int) + bufferUTF8.Length; //获取总大小
            UserToken.SendBuffer.StartPacket();
            UserToken.SendBuffer.m_dynamicBufferManager.WriteInt(totalLength); //写入总大小
            UserToken.SendBuffer.m_dynamicBufferManager.WriteBuffer(bufferUTF8); //写入命令内容
            UserToken.SendBuffer.EndPacket();

            int packetOffset = 0;
            int packetCount = 0;
            if (UserToken.SendBuffer.GetFirstPacket(ref packetOffset, ref packetCount))
            {
                m_tcpClient.Client.Send(UserToken.SendBuffer.m_dynamicBufferManager.Buffer, 0, UserToken.SendBuffer.m_dynamicBufferManager.DataCount, SocketFlags.None); //使用阻塞模式，Socket会一次发送完所有数据后才返回
                while (UserToken.SendBuffer.GetFirstPacket(ref packetOffset, ref packetCount))
                {
                    UserToken.SendBuffer.ClearFirstPacket();
                    m_tcpClient.Client.Send(UserToken.SendBuffer.m_dynamicBufferManager.Buffer, 0, UserToken.SendBuffer.m_dynamicBufferManager.DataCount, SocketFlags.None); //使用阻塞模式，Socket会一次发送完所有数据后才返回    
                }
            }
            return true;
        }
    }
}
