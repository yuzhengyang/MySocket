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

namespace SocketClientCommonLib
{
    /// <summary>
    /// 委托回调函数 this.Invoke(new ThreadStart(delegate{})) 实现与UI交换
    /// </summary>
    public class DelegateState
    {

        public delegate void SocketStateCallBack(string msg);
        /// <summary>
        /// 信息显示
        /// </summary>
        public static SocketStateCallBack ServerStateInfo;
    }
}
