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
    /// 信息转发模式(独特的转发没有经过组装的信息)
    /// </summary>
    public class MesgTransmitSocketProtocol : SocketInvokeElement
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="asyncSocketServer">AsyncSocketServer</param>
        /// <param name="socketUserToken">SocketUserToken</param>
        public MesgTransmitSocketProtocol(AsyncSocketServer asyncSocketServer, SocketUserToken socketUserToken)
            : base(asyncSocketServer, socketUserToken)
        {
        }

        /// <summary>
        /// 重写ProcessReceive  信息直接发送
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset">int</param>
        /// <param name="count">int</param>
        /// <returns>bool</returns>
        public override bool ProcessReceive(byte[] buffer, int offset, int count) //接收异步事件返回的数据，用于对数据进行缓存和分包
        {
            socketUserToken.ActiveDateTime = DateTime.Now;
            if (socketUserToken.SendUserTokenAll.Count > 0)
                if (socketUserToken.SendUserTokenAll[0].ConnectSocket != null)
                {
                    AsyncSendBufferManager asyncSendBufferManager = socketUserToken.SendBuffer;
                    asyncSendBufferManager.StartPacket();
                    asyncSendBufferManager.m_dynamicBufferManager.WriteInt(buffer.Length); //写入总大小
                    asyncSendBufferManager.m_dynamicBufferManager.WriteBuffer(buffer); //写入命令内容
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
            return true;
        }
    }
}
