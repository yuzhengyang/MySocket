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
using System.Security.Cryptography;
using System.Text;

namespace HelpCommonLib
{
    /// <summary>
    /// 原生态Encrypt加密
    /// </summary>
    public class Encrypt
    {
        private byte[] iba_mIV = new byte[8];  //向量
        private byte[] iba_mKey = new byte[8]; //密钥
        private DESCryptoServiceProvider io_DES = new DESCryptoServiceProvider();

        public Encrypt()
        {
            this.iba_mKey[0] = 0x95;
            this.iba_mKey[1] = 0xc4;
            this.iba_mKey[2] = 0xf6;
            this.iba_mKey[3] = 0x49;
            this.iba_mKey[4] = 0xac;
            this.iba_mKey[5] = 0x61;
            this.iba_mKey[6] = 0xa3;
            this.iba_mKey[7] = 0xe2;
            this.iba_mIV[0] = 0xf9;
            this.iba_mIV[1] = 0x6a;
            this.iba_mIV[2] = 0x65;
            this.iba_mIV[3] = 0xb8;
            this.iba_mIV[4] = 0x4a;
            this.iba_mIV[5] = 0x23;
            this.iba_mIV[6] = 0xfe;
            this.iba_mIV[7] = 0xc6;
            this.io_DES.Key = this.iba_mKey;
            this.io_DES.IV = this.iba_mIV;
        }
        /// <summary>
        /// 初始化加密向量与密钥 长度为8
        /// </summary>
        /// <param name="iba_mIV">向量</param>
        /// <param name="iba_mKey">密钥</param>
        public Encrypt(byte[] iba_mIV, byte[] iba_mKey)
        {
            this.io_DES.IV = iba_mIV;
            this.io_DES.Key = iba_mKey;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="as_Data"></param>
        /// <returns></returns>
        public string doDecrypt(string as_Data)
        {
            ICryptoTransform lo_ICT = this.io_DES.CreateDecryptor(this.io_DES.Key, this.io_DES.IV);
            try
            {
                byte[] lba_bufIn = this.FromHexString(as_Data);//Encoding.UTF8.GetString(Convert.FromBase64String(
                byte[] lba_bufOut = lo_ICT.TransformFinalBlock(lba_bufIn, 0, lba_bufIn.Length);
                return Encoding.UTF8.GetString(lba_bufOut);
            }
            catch
            {
                return as_Data;
            }
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="as_Data"></param>
        /// <returns></returns>
        public string doEncrypt(string as_Data)
        {
            ICryptoTransform lo_ICT = this.io_DES.CreateEncryptor(this.io_DES.Key, this.io_DES.IV);
            try
            {
                byte[] lba_bufIn = Encoding.UTF8.GetBytes(as_Data);
                byte[] lba_bufOut = lo_ICT.TransformFinalBlock(lba_bufIn, 0, lba_bufIn.Length);
                return GetHexString(lba_bufOut);//Convert.ToBase64String(Encoding.UTF8.GetBytes();
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 转换2进制
        /// </summary>
        /// <param name="as_value"></param>
        /// <returns></returns>
        private byte[] FromHexString(string as_value)
        {
            byte[] lba_buf = new byte[Convert.ToInt32((int)(as_value.Length / 2))];
            for (int li_i = 0; li_i < lba_buf.Length; li_i++)
            {
                lba_buf[li_i] = Convert.ToByte(as_value.Substring(li_i * 2, 2), 0x10);
            }
            return lba_buf;
        }
        /// <summary>
        /// 字节转字符串
        /// </summary>
        /// <param name="aba_buf"></param>
        /// <returns></returns>
        private string GetHexString(byte[] aba_buf)
        {
            StringBuilder lsb_value = new StringBuilder();
            foreach (byte lb_byte in aba_buf)
            {
                lsb_value.Append(Convert.ToString(lb_byte, 0x10).PadLeft(2, '0'));
            }
            return lsb_value.ToString();
        }
    }
}
