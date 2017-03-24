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

namespace SocketClientCommonLib
{
    /// <summary>
    /// 输出时信息组装
    /// </summary>
    public class AssemblyOutDataParser
    {
        private List<string> m_protocolText;

        /// <summary>
        /// 初始化协议
        /// </summary>
        public AssemblyOutDataParser()
        {
            m_protocolText = new List<string>();
        }

        /// <summary>
        /// 清楚以前协议
        /// </summary>
        public void Clear()
        {
            m_protocolText.Clear();
        }
        /// <summary>
        /// 发送信息通过协议组装后的信息
        /// </summary>
        /// <returns>string</returns>
        public string GetProtocolText()
        {
            string tmpStr = "";
            if (m_protocolText.Count > 0)
            {
                tmpStr = m_protocolText[0];
                for (int i = 1; i < m_protocolText.Count; i++)
                {
                    tmpStr += m_protocolText[i];
                }
            }
            return tmpStr;
        }

        /// <summary>
        /// 协议组装Key只和Value值
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <param name="value"></param>
        public void AddValue(string protocolKey, string value)
        {
            m_protocolText.Add(protocolKey + "=" + value.ToString());
        }
        /// <summary>
        /// 添加返回头部
        /// </summary>
        public void AddRequest()
        {
            m_protocolText.Add(ProtocolKeys.LeftBrackets + ProtocolKeys.Request + ProtocolKeys.RightBrackets);
        }

        /// <summary>
        /// 添加成功参数
        /// </summary>
        public void AddSuccess()
        {
            m_protocolText.Add(ProtocolKeys.Code + ProtocolKeys.EqualSign + ProtocolCodes.Success.ToString());
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="commandKey"></param>
        public void AddCommand(string commandKey)
        {
            m_protocolText.Add(ProtocolKeys.Command + ProtocolKeys.EqualSign + commandKey);
        }
        /// <summary>
        /// 填入提交头部
        /// </summary>
        public void AddResponse()
        {
            m_protocolText.Add(ProtocolKeys.LeftBrackets + ProtocolKeys.Response + ProtocolKeys.RightBrackets);
        }
    }
}
