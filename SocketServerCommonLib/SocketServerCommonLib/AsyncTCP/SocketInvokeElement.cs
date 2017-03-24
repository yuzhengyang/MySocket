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
    /// 信息处理-信息发送接收过滤器
    /// </summary>
    public class SocketInvokeElement
    {
        protected AsyncSocketServer m_asyncSocketServer;
        //客户端
        private SocketUserToken m_socketUserToken;
        protected SocketUserToken socketUserToken {get { return m_socketUserToken; }  set { m_socketUserToken = value; }  }

        //是否有发送异步事件
        protected bool m_sendAsync;
        //过滤器创建时间
        protected DateTime m_connectDT;
        public DateTime ConnectDT { get { return m_connectDT; } }
        //过滤器最后操作时间
        protected DateTime m_activeDT;
        public DateTime ActiveDT { get { return m_activeDT; } }


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
        /// 初始化
        /// </summary>
        /// <param name="asyncSocketServer"></param>
        /// <param name="socketUserToken"></param>
        public SocketInvokeElement(AsyncSocketServer asyncSocketServer, SocketUserToken socketUserToken)
        {
            m_asyncSocketServer = asyncSocketServer;
            m_socketUserToken = socketUserToken;

            m_outDataParser = new AssemblyOutDataParser();
            m_InDataParser = new AssemblyInDataParser();
            m_sendAsync = false;
            m_connectDT = DateTime.UtcNow;
            m_activeDT = DateTime.UtcNow;
        }

        /// <summary>
        /// 协议组装/继续接收
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual bool ProcessReceive(byte[] buffer, int offset, int count) //接收异步事件返回的数据，用于对数据进行缓存和分包
        {
            m_activeDT = DateTime.UtcNow;
            DynamicBufferManager receiveBuffer = m_socketUserToken.ReceiveBuffer;
            receiveBuffer.WriteBuffer(buffer, offset, count);//把刚刚接收的信息加入到缓存中
            bool result = true;

            //按照长度分包
            int packetLength = BitConverter.ToInt32(receiveBuffer.Buffer, 0); //获取包长度
            if ((packetLength > 10 * 1024 * 1024) | (receiveBuffer.DataCount > 10 * 1024 * 1024)) //最大Buffer异常保护
                return false;

            if (receiveBuffer.DataCount >= packetLength) //packetLength收到的数据达到包长度
            {
                //命令 组装-如果组装失败则继续接收：如果组装成功-则删除
                result = ProcessPacket(receiveBuffer.Buffer, offset, packetLength);
                if (result)
                    receiveBuffer.Clear(packetLength);//清理已经处理的协议
                else
                    return result;
            }
            return true;
        }
        /// <summary>
        /// 组装命令协议
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual bool ProcessPacket(byte[] buffer, int offset, int count) //处理分完包后的数据，把命令和数据分开，并对命令进行解析
        {
            string tmpStr = Encoding.UTF8.GetString(buffer, offset, count);
            //命令组装或者查询
            if (!m_InDataParser.DecodeProtocolText(tmpStr))
                return false;

            return ProcessCommand(buffer, offset, count); //处理命令
        }
        /// <summary>
        /// 协议通过发送客户端信息(被子类重写-执行子类的方法)
        /// </summary>
        /// <param name="buffer">接收到的信息</param>
        /// <param name="offset">从第几位开始</param>
        /// <param name="count">共有几位</param>
        /// <returns>true</returns>
        public virtual bool ProcessCommand(byte[] buffer, int offset, int count)
        {
            return true;
        }

        /// <summary>
        /// 发送回调函数
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public virtual bool SendCompleted(SocketUserToken userToken)
        {
            userToken.m_sendAsync = false;
            AsyncSendBufferManager asyncSendBufferManager = userToken.SendBuffer;
            asyncSendBufferManager.ClearFirstPacket(); //清除已发送的包
            int offset = 0;
            int count = 0;
            if (asyncSendBufferManager.GetFirstPacket(ref offset, ref count))//缓存中是否还有数据没有发送,有就继续发送
            {
                userToken.m_sendAsync = true;
                return m_asyncSocketServer.SendAsyncEvent(userToken.ConnectSocket, userToken.SendEventArgs,
                    asyncSendBufferManager.m_dynamicBufferManager.Buffer, offset, count);
            }
            else
                return true;
        }
    }
}
