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
using System.Net.Mail;

namespace HelpCommonLib
{
    public class ComminClass
    {
        #region 邮件发送
        /// <summary>
        /// 功能：发送邮件,返回字符串：“发送成功”否则返回错误代码。
        /// </summary>
        /// <param name="MailTo">MailTo为收信人地址</param>
        /// <param name="Subject">Subject为标题</param>
        /// <param name="Body">Body为信件内容</param>
        /// <param name="BodyFormat">BodyFormat为信件内容格式:0为Text,1为Html</param>
        /// <param name="Priority">Priority为优先级:0为低,1为中,2为高</param>
        /// <param name="Attachments">Attachment为附件,为null则不发送</param>
        public static string SendMail(System.Collections.ArrayList MailTo, string Subject, string Body, int BodyFormat, int Priority, string Attachments)
        {
            string result;

            SmtpClient mail = new SmtpClient();
            //发送方式
            mail.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtp服务器
            mail.Host = "smtp.xxxx.com";
            //用户名凭证               
            mail.Credentials = new System.Net.NetworkCredential("xxx@xxx.com", "xxxxxx");
            //邮件信息
            MailMessage message = new MailMessage();
            //发件人
            message.From = new MailAddress("xxx@xxxxxx.com");
            //收件人
            foreach (object item in MailTo)
            {
                message.To.Add(item.ToString());
            }
            //主题
            message.Subject = Subject;
            //内容
            message.Body = Body;
            //正文编码
            message.BodyEncoding = System.Text.Encoding.UTF8;
            //设置为HTML格式
            message.IsBodyHtml = true;
            //优先级
            message.Priority = MailPriority.High;
            try
            {
                mail.Send(message);
                result = "发送成功";
                return result;
            }
            catch (Exception e)
            {
                result = e.ToString();
            }
            return result;
        }
        #endregion

        #region md5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <param name="length">16或32值之一</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(string strSource, int length)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(strSource);
            byte[] hashValue = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            switch (length)
            {
                case 16:
                    for (int i = 4; i < 12; i++)
                        sb.Append(hashValue[i].ToString("x2"));
                    break;
                case 32:
                    for (int i = 0; i < 16; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
                default:
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
            }
            return sb.ToString();
        }
        #endregion
    }
}
