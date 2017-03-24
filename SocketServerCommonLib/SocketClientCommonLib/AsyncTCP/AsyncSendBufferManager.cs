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

namespace SocketClientCommonLib
{

    /// <summary>
    /// 异步发送，有时,可能接收到多个命令,发送需要等待上一次(回调函数)才发下一次的响应
    /// </summary>
    public class AsyncSendBufferManager
    {
        /// <summary>
        /// 发送包的位置及大小
        /// </summary>
        struct SendBufferPacket
        {
            public int Offset;
            public int Count;
        }

        /// <summary>
        /// 接收区缓存
        /// </summary>
        public DynamicBufferManager m_dynamicBufferManager { get; set; }
        private List<SendBufferPacket> m_sendBufferList;//异步发送时多个包集合,连续一个一个的发送
        private SendBufferPacket m_sendBufferPacket;//发送记录包

        /// <summary>
        /// 初始化发送缓存
        /// </summary>
        /// <param name="bufferSize">int</param>
        public AsyncSendBufferManager(int bufferSize)
        {
            m_dynamicBufferManager = new DynamicBufferManager(bufferSize);
            m_sendBufferList = new List<SendBufferPacket>();
            m_sendBufferPacket.Offset = 0;
            m_sendBufferPacket.Count = 0;
        }

        /// <summary>
        /// 发送时 先确认历史包的大小 
        /// 匹配使用 EndPacket()
        /// </summary>
        public void StartPacket()
        {
            m_sendBufferPacket.Offset = m_dynamicBufferManager.DataCount;  // m_dynamicBufferManager.DataCount历史缓存位置
            m_sendBufferPacket.Count = 0;
        }
        /// <summary>
        /// 数据填写按成时,确定数据包的大小和位置(x至x)
        /// StartPacket()匹配使用
        /// </summary>
        public void EndPacket()
        {
            m_sendBufferPacket.Count = m_dynamicBufferManager.DataCount - m_sendBufferPacket.Offset;//包的大小
            m_sendBufferList.Add(m_sendBufferPacket);
        }
        /// <summary>
        /// 是否还有包(有包则继续发送)
        /// (检测包是否发送完毕,如果没有则返回True)
        /// </summary>
        /// <param name="offset">包的位置(清除了第一个包后，后续的包往前移，因此Offset都为0)</param>
        /// <param name="count">需要发送包的大小</param>
        /// <returns>bool</returns>
        public bool GetFirstPacket(ref int offset, ref int count)
        {
            if (m_sendBufferList.Count <= 0)
                return false;
            offset = 0;
            count = m_sendBufferList[0].Count;
            return true;
        }
        /// <summary>
        /// 清除已经发送的包
        /// </summary>
        /// <returns>bool</returns>
        public bool ClearFirstPacket()
        {
            if (m_sendBufferList.Count <= 0)
                return false;
            int count = m_sendBufferList[0].Count;
            m_dynamicBufferManager.Clear(count);
            m_sendBufferList.RemoveAt(0);
            return true;
        }
        /// <summary>
        /// 发送全部清空
        /// </summary>
        public void ClearPacket()
        {
            m_sendBufferList.Clear();
            m_dynamicBufferManager.Clear(m_dynamicBufferManager.DataCount);
        }

        /// <summary>
        /// 获取剩余包
        /// </summary>
        /// <returns>int</returns>
        public int GetsendbufferListCount()
        {
            return m_sendBufferList.Count;
        }
    }
}
