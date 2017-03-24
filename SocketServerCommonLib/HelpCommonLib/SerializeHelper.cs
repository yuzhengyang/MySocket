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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HelpCommonLib
{
    /// <summary>
    /// 序列化帮助类(需要序列化的类上加[Serializable])
    /// </summary>
    public class SerializeHelper
    {
        /// <summary>
        /// 序列化一个对象
        /// </summary>
        /// <param name="o">将要序列化的对象</param>
        /// <returns>返回byte[]</returns>
        public static byte[] Serialize(object o)
        {
            if (o == null) return null;
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, o);
            ms.Position = 0;
            byte[] b = new byte[ms.Length];
            ms.Read(b, 0, b.Length);
            ms.Close();
            return b;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="b">返回一个对象</param>
        public static object Deserialize(byte[] b)
        {
            if (b.Length == 0) return null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                ms.Write(b, 0, b.Length);
                ms.Position = 0;
                object n = (object)bf.Deserialize(ms);
                ms.Close();
                return n;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }

    }

}
