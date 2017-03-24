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
using System.Text;
using System.Security.Cryptography;

namespace HelpCommonLib
{

    /// <summary>
    /// DES加密 / 解密 8密钥
    /// </summary>
    public class DESEncrypt
    {

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">字符串</param>
        /// <returns>string</returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "Amos..Li");
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>string</returns>
        public static string Encrypt(string Text, string sKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray;
                inputByteArray = Encoding.Default.GetBytes(Text);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch
            {
                throw new Exception("Encrypt Error!");
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">字符串</param>
        /// <returns>string</returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "Amos..Li");
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text">字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>string</returns>
        public static string Decrypt(string Text, string sKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int len;
                len = Text.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                throw new Exception("Decrypt Error!");
            }
        }

    }
}
