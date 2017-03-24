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
using HelpCommonLib;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 接受到信息时信息组装
    /// </summary>
    public class AssemblyInDataParser
    {
        //信息头
        private string m_header;
        public string Header { get { return m_header; } }
        //信息命令
        private string m_command;
        public string Command { get { return m_command; } }
        //信息名字
        private List<string> m_names;
        public List<string> Names { get { return m_names; } }
        //信息
        private List<string> m_values;
        public List<string> Values { get { return m_values; } }

        /// <summary>
        /// 初始化信息集合
        /// </summary>
        public AssemblyInDataParser()
        {
            m_names = new List<string>();
            m_values = new List<string>();
        }


        /// <summary>
        /// 信息组装
        /// </summary>
        /// <param name="protocolText"></param>
        /// <returns></returns>
        public bool DecodeProtocolText(string protocolText)
        {
            //再次组装前，先清除上一次信息
            m_header = "";
            m_names.Clear();
            m_values.Clear();
            int speIndex = protocolText.IndexOf(ProtocolKeys.ReturnWrap);
            if (speIndex < 0)
                return false;
            else
            {
                //协议组装
                string[] tmpNameValues = protocolText.Split(new string[] { ProtocolKeys.ReturnWrap }, StringSplitOptions.RemoveEmptyEntries);
                if (tmpNameValues.Length < 2) //每次命令至少包括两行
                    return false;
                for (int i = 0; i < tmpNameValues.Length; i++)//解析
                {
                    string[] tmpStr = tmpNameValues[i].Split(new string[] { ProtocolKeys.EqualSign }, StringSplitOptions.None);
                    if (tmpStr.Length > 1) //存在等号
                    {
                        if (tmpStr.Length > 2) //超过两个等号，返回失败
                            return false;
                        if (tmpStr[0].Equals(ProtocolKeys.Command, StringComparison.CurrentCultureIgnoreCase))//找到命令
                        {
                            m_command = tmpStr[1];
                            DelegateState.ServerStateInfo("Command命令:" + m_command);
                        }
                        else
                        {
                            m_names.Add(tmpStr[0].ToLower());//（当多条命令需要一次处理时候,需要）
                            m_values.Add(tmpStr[1]);
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 根据协议名称查询值(当多条命令需要一次处理时候，请用此方法)
        /// </summary>
        /// <param name="protocolKey">string 协议名称</param>
        /// <returns>List[string]信息</returns>
        public List<string> GetValue(string protocolKey)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < m_names.Count; i++)
            {
                if (protocolKey.Equals(m_names[i], StringComparison.CurrentCultureIgnoreCase))
                    result.Add(m_values[i]);
            }
            return result;
        }

        /// <summary>
        /// 根据协议名称查询值
        /// </summary>
        /// <param name="protocolKey">协议名称</param>
        /// <param name="value">ref 值</param>
        /// <returns>是否存在</returns>
        public bool GetValue(string protocolKey, ref string value)
        {
            int index = m_names.IndexOf(protocolKey.ToLower());
            if (index > -1)
            {
                value = m_values[index];
                return true;
            }
            else
                return false;
        }
    }
}
