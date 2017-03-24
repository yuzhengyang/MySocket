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

namespace SocketServerCommonLib
{
    public class AsyncSocketUserPool
    {
        /// <summary>
        /// 使用用户池-(高并发处理,减轻松服务器压力)
        /// 每一次客户端连接的时候,都是从用户池子里面取一个空用户(稳定),而不是重新去开辟一个空间
        /// 避免SocketUserToken 多次重复在内存中创建,删除
        /// 需要服务启动的时候初始化
        /// </summary>
        private Stack<SocketUserToken> m_pool;

        public AsyncSocketUserPool(int capacity)
        {
            m_pool = new Stack<SocketUserToken>(capacity);
        }

        /// <summary>
        /// 把  SocketUserToken =null  的重新取出来加入列队
        /// </summary>
        /// <param name="item">SocketUserToken</param>
        public void Push(SocketUserToken item)
        {
            if (item == null)
            {
                throw new ArgumentException("项目的用户不能为空！");
            }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }
       /// <summary>
        /// 分配SocketUserToken一个地址给你
       /// </summary>
        /// <returns>SocketUserToken</returns>
        public SocketUserToken Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }
        /// <summary>
        /// 用户池大小
        /// </summary>
        public int Count
        {
            get { return m_pool.Count; }
        }
    }

    /// <summary>
    /// 实时用户集合
    /// </summary>
    public class SocketUserClientList : Object
    {
        private List<SocketUserToken> m_userlist;

        public SocketUserClientList()
        {
            m_userlist = new List<SocketUserToken>();
        }

        /// <summary>
        /// 用户集合
        /// </summary>
        public List<SocketUserToken> Userlist
        {
            get { return m_userlist; }
            set { m_userlist = value; }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userToken">用户信息</param>
        public void Add(SocketUserToken userToken)
        {
            lock (m_userlist)
            {
                m_userlist.Add(userToken);
            }
        }
        /// <summary>
        /// 删除一个用户SocketUserToken
        /// </summary>
        /// <param name="userToken">SocketUserToken</param>
        public void Remove(SocketUserToken userToken)
        {
            lock (m_userlist)
            {
                m_userlist.Remove(userToken);
            }
        }
        /// <summary>
        /// 复制给SocketUserToken[]
        /// </summary>
        /// <param name="array">ref SocketUserToken[]</param>
        public void CopyList(ref SocketUserToken[] array)
        {
            lock (m_userlist)
            {
                array = new SocketUserToken[m_userlist.Count];
                m_userlist.CopyTo(array);
            }
        }
    }

    /// <summary>
    /// 设备客户端(物联网时确定设备)登录时判断是否是设备
    /// </summary>
    public class SocketDeviceClientList : Object
    {
        private List<SocketUserToken> m_Devicelist;

        public SocketDeviceClientList()
        {
            m_Devicelist = new List<SocketUserToken>();
        }

        /// <summary>
        /// 用户集合
        /// </summary>
        public List<SocketUserToken> Devicelist
        {
            get { return m_Devicelist; }
            set { m_Devicelist = value; }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userToken">用户信息</param>
        public void Add(SocketUserToken userToken)
        {
            lock (m_Devicelist)
            {
                m_Devicelist.Add(userToken);
            }
        }
        /// <summary>
        /// 删除一个用户SocketUserToken
        /// </summary>
        /// <param name="userToken">SocketUserToken</param>
        public void Remove(SocketUserToken userToken)
        {
            lock (m_Devicelist)
            {
                m_Devicelist.Remove(userToken);
            }
        }
        /// <summary>
        /// 复制给SocketUserToken[]
        /// </summary>
        /// <param name="array">ref SocketUserToken[]</param>
        public void CopyList(ref SocketUserToken[] array)
        {
            lock (m_Devicelist)
            {
                array = new SocketUserToken[m_Devicelist.Count];
                m_Devicelist.CopyTo(array);
            }
        }
    }

}
