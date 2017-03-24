using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
namespace HelpCommonLib
{
    /// <summary>
    /// WEBQQ
    /// </summary>
    public class WebQQ
    {
        /// <summary>
        /// 初始化QQ
        /// </summary>
        /// <param name="qq">QQ帐号</param>
        /// <param name="pass">QQ密码</param>
        /// <param name="qs">QQ状态</param>
        public WebQQ(string qq, string pass, QQStatus qs = QQStatus.online)
        {
            user.QQID = qq;
            user.QQPass = pass;
            user.State = qs;
            user.ClientID = GetClientID();//取客户端ID
        }

        #region 变量
        /// <summary>
        /// 全局wininet
        /// </summary>
        XJHTTP qwin = new XJHTTP();
        /// <summary>
        /// 用户数据
        /// </summary>
        public UserInfo user = new UserInfo();
        /// <summary>
        /// 心跳线程
        /// </summary>
        Thread PollThread;
        /// <summary>
        /// 暂停心跳
        /// </summary>
        public bool StopPoll = false;

        #endregion

        #region Get方法
        /// <summary>
        /// 判断是否需要验证码
        /// </summary>
        public int GetCheckVC()
        {
            string html = qwin.GetHtmlByWininet("https://ssl.ptlogin2.qq.com/check?uin=" + user.QQID + "&appid=501004106");
            MatchCollection rg = Regex.Matches(html, "ptui_checkVC\\('(\\d+)','(.*?)','(.*?)'");
            if (rg.Count != 0)
            {
                string state = rg[0].Groups[1].Value;
                user.Vcode = rg[0].Groups[2].Value;
                return int.Parse(state);
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <returns></returns>
        public Image GetCodeImg()
        {
            Image img = qwin.GetImageByWininet("https://ssl.captcha.qq.com/getimage?aid=501004106&uin=" + user.QQID);
            return img;
        }
        /// <summary>
        /// 获取用户头像 40X40
        /// </summary>
        /// <returns>验证码图片</returns>
        public Image GetHeadImg()
        {
            Image img = qwin.GetImageByWininet("http://face7.web.qq.com/cgi/svr/face/getface?cache=1&type=11&fid=0&uin=" + user.QQID + "&vfwebqq=" + user.Vfwebqq);
            return img;
        }
        /// <summary>
        /// 获取用户签名
        /// </summary>
        /// <returns></returns>
        public string GetSign()
        {
            string html = qwin.GetHtmlByWininetUTF8("http://s.web2.qq.com/api/get_single_long_nick2?tuin=" + user.QQID + "&vfwebqq=" + user.Vfwebqq);
            return Tools.RegexHelp(html, "lnick\":\"(.*?)\"");
        }
        /// <summary>
        /// 获取好友列表
        /// </summary>
        public List<FriendsInfo> GetFriends()
        {
            string html = qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/get_user_friends2", "r=%7B%22h%22%3A%22hello%22%2C%22hash%22%3A%22" + GetHash(user.QQID, user.Ptwebqq) + "%22%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
            ArrayList list = new ArrayList();
            list.Add(JsonHelp.GetJson(html, "result", "info"));
            List<FriendsInfo> ls = new List<FriendsInfo>();
            for (int i = 0; i < list.Count; i++)
            {
                FriendsInfo fi = new FriendsInfo();
                Dictionary<string, object> dic = ((Dictionary<string, object>)list[i]);
                if (dic.ContainsKey("nick") && dic.ContainsKey("uin"))
                {
                    fi.Nick = dic["nick"].ToString();
                    fi.Uin = dic["uin"].ToString();
                }
                ls.Add(fi);
            }
            return ls;
        }
        /// <summary>
        /// 获取群列表
        /// </summary>
        public List<GroupInfo> GetGroups()
        {
            string html = qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/get_group_name_list_mask2", "r=%7B%22hash%22%3A%22" + GetHash(user.QQID, user.Ptwebqq) + "%22%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
            ArrayList list =new ArrayList();
            list.Add(JsonHelp.GetJson(html, "result", "gnamelist"));
            List<GroupInfo> ls = new List<GroupInfo>();
            for (int i = 0; i < list.Count; i++)
            {
                GroupInfo gi = new GroupInfo();
                Dictionary<string, object> dic = ((Dictionary<string, object>)list[i]);
                if(dic.ContainsKey("name")){
                gi.Name = dic["name"].ToString();}
                if (dic.ContainsKey("code"))
                {
                gi.Code = dic["code"].ToString();}
                if (dic.ContainsKey("gid"))
                {
                    gi.GID = dic["gid"].ToString();
                }
                ls.Add(gi);
            }
            return ls;
        }
        /// <summary>
        /// 获取真实好友号和群号
        /// </summary>
        /// <param name="uin">需要识别的id</param>
        /// <param name="type">识别的类型1=好友、4=群</param>
        public string GetTrueUin(string uin, string type)
        {
            string json = qwin.GetHtmlByWininetUTF8("http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&verifysession=&type=" + type + "&code=&vfwebqq=" + user.Vfwebqq);
            return Tools.RegexHelp(json, "account\":(\\d+),");
        }
        #endregion

        #region UP方法
        /// <summary>
        /// 修改在线状态
        /// </summary>
        public void UpStatus()
        {
            qwin.GetHtmlByWininetUTF8("http://d.web2.qq.com/channel/change_status2?newstatus=" + user.State + "&clientid=" + user.ClientID + "&psessionid=" + user.Psessionid);
        }

        /// <summary>
        /// 修改用户签名
        /// </summary>
        /// <param name="sg">修改的签名</param>
        public void UpSign(string sg)
        {
            qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/set_long_nick2", "r=%7B%22nlk%22%3A%22" + HttpUtility.UrlEncode(sg) + "%22%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
        }
        #endregion

        /// <summary>
        /// 登录QQ
        /// </summary>
        /// <returns>登录返回的信息</returns>
        public string Login()
        {
           
         
            //登录验证
            string html = qwin.GetHtmlByWininetUTF8("https://ssl.ptlogin2.qq.com/login?u=" + user.QQID + "&p=" + GetPass(user.QQID, user.QQPass, user.Vcode.ToUpper()) + "&verifycode=" + user.Vcode.ToUpper() + "&webqq_type=10&remember_uin=1&login2qq=1&aid=1003903&u1=http%3A%2F%2Fweb2.qq.com%2Floginproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&mibao_css=m_webqq&t=2&g=1&js_type=0&js_ver=10071");
            MatchCollection rg = Regex.Matches(html, "ptuiCB\\('(\\d+)','\\d+','(.*?)','\\d+','(.*?)', '(.*?)'\\);");
            if (rg.Count != 0)
            {
                string stu = rg[0].Groups[1].Value;
                string jurl = rg[0].Groups[2].Value;
                string rstr = rg[0].Groups[3].Value;
                user.QQNick = rg[0].Groups[4].Value;//获取昵称
                if (stu == "0")
                {
                    qwin.GetHtmlByWininet(jurl);
                    user.Ptwebqq = Tools.RegexHelp(qwin.GetCookieByWininet("http://qq.com"), "ptwebqq=(.*?);");
                    //真正登录上线 
                    html = qwin.POSTHtmlByWininetUTF8("http://d.web2.qq.com/channel/login2", "r=%7B%22ptwebqq%22%3A%22" + user.Ptwebqq + "%22%2C%22clientid%22%3A" + user.ClientID + "%2C%22psessionid%22%3A%22%22%2C%22status%22%3A%22" + user.State + "%22%7D");
                    //取得下面2个令牌
                    user.Vfwebqq = Tools.RegexHelp(html, "vfwebqq\":\"(.*?)\"");
                    user.Psessionid = Tools.RegexHelp(html, "psessionid\":\"(.*?)\"");
                    return rstr;
                }
                else
                {
                    return rstr;
                }
            }
            else
            {
                return html;
            }
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="uin">临时UIN</param>
        /// <param name="type">是否从对方列表中删除</param>
        public void DeleteFriends(string uin, bool type)
        {
            string dt = type ? "2" : "1";
            qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/delete_friend", "tuin=" + uin + "&delType=" + dt + "&vfwebqq=" + user.Vfwebqq);
        }

        /// <summary>
        /// 踢出群成员
        /// </summary>
        /// <param name="qun">Q群真实号码</param>
        /// <param name="qq">群成员真实号码</param>
        public void Kicking(string qun, string qq)
        {
            string cookie = qwin.GetCookieByWininet("http://qq.com");
            string bkn = G_tk(Tools.RegexHelp(cookie, "skey=(.*?);"));
            string postdata = "gc=" + qun + "&bkn=" + bkn + "&ul=" + qq;
            string xxx = qwin.POSTHtmlByWininetUTF8("http://qinfo.clt.qq.com/cgi-bin/qun_info/delete_group_member", postdata);
        }

        /// <summary>
        /// 处理好友请求
        /// </summary>
        /// <param name="type">0-不操作 1-同意并添加 2-同意 3-拒绝</param>
        /// <param name="qq">真实QQ号</param>
        /// <param name="reason">拒绝理由</param>
        public void DoFriendRequest(int type,string qq, string reason="")
        {
            switch (type)
            {
                case 0://不处理
                    break;
                case 1://同意并添加为好友
                    qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/allow_and_add2", "r=%7B%22account%22%3A" + qq + "%2C%22gid%22%3A0%2C%22mname%22%3A%22%22%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
                    break;
                case 2://仅同意
                    qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/allow_added_request2", "r=%7B%22account%22%3A" + qq + "%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
                    break;
                case 3://拒绝
                    qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/deny_added_request2", "r=%7B%22account%22%3A" + qq + "%2C%22msg%22%3A%22" + HttpUtility.UrlEncode(reason) + "%22%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
                    break;
            }
        }

        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="uin">群的CODE</param>
        public void DeleteGroup(string uin)
        {
            qwin.POSTHtmlByWininetUTF8("http://s.web2.qq.com/api/quit_group2", "r=%7B%22gcode%22%3A%22" + uin + "%22%2C%22vfwebqq%22%3A%22" + user.Vfwebqq + "%22%7D");
        }

        /// <summary>
        /// 掉线重登
        /// </summary>
        public void Login2(QQStatus qs)
        {
            string html = qwin.POSTHtmlByWininetUTF8("http://d.web2.qq.com/channel/login2", "r=%7B%22status%22%3A%22" + qs.ToString() + "%22%2C%22ptwebqq%22%3A%22" + user.Ptwebqq + "%22%2C%22passwd_sig%22%3A%22%22%2C%22clientid%22%3A%22" + user.ClientID + "%22%2C%22psessionid%22%3A%22" + user.Psessionid + "%22%7D&clientid=" + user.ClientID + "&psessionid=" + user.Psessionid);
            //取得下面2个令牌
            user.Vfwebqq = Tools.RegexHelp(html, "vfwebqq\":\"(.*?)\"");
            user.Psessionid = Tools.RegexHelp(html, "psessionid\":\"(.*?)\"");
        }

        /// <summary>
        /// 发送QQ消息
        /// </summary>
        /// <param name="type">1=好友、2=群、3=临时会话</param>
        /// <param name="uin">临时消息号（群OR好友）</param>
        /// <param name="id">临时会话ID</param>
        /// <param name="content">发送的内容</param>
        public void SendMessage(int type, string uin, string id, string content)
        {
            string b = user.Bold ? "1" : "0";
            string i = user.Italic ? "1" : "0";
            string u = user.Underline ? "1" : "0";

            string url = string.Empty;
            string heard = string.Empty;
            switch (type)
            {
                case 1://发给好友
                    url = "http://d.web2.qq.com/channel/send_buddy_msg2";
                    heard = "r=%7B%22to%22%3A" + uin + "%2C%22face%22%3A522%2C%22content%22%3A%22%5B";
                    break;
                case 2://发给群
                    url = "http://d.web2.qq.com/channel/send_qun_msg2";
                    heard = "r=%7B%22group_uin%22%3A" + uin + "%2C%22content%22%3A%22%5B";
                    break;
                case 3://发给临时会话
                    url = "http://d.web2.qq.com/channel/send_sess_msg2";
                    heard = "r=%7B%22to%22%3A" + uin + "%2C%22group_sig%22%3A%22" + GetGroupSig(id, uin) + "%22%2C%22face%22%3A561%2C%22content%22%3A%22%5B";
                    break;
                default:
                    return;
            }

            string postdata = heard + HttpUtility.UrlEncode(MakeContent(content)) + "%5C%22%5C%22%2C%5B%5C%22font%5C%22%2C%7B%5C%22name%5C%22%3A%5C%22" + HttpUtility.UrlEncode(user.QQFont) + "%5C%22%2C%5C%22size%5C%22%3A%5C%22" + user.FontSize + "%5C%22%2C%5C%22style%5C%22%3A%5B" + b + "%2C" + i + "%2C" + u + "%5D%2C%5C%22color%5C%22%3A%5C%22" + user.FontColor.ToArgb().ToString("X8").Substring(2, 6) + "%5C%22%7D%5D%5D%22%2C%22msg_id%22%3A" + GetMsgID() + "%2C%22clientid%22%3A%22" + user.ClientID + "%22%2C%22psessionid%22%3A%22" + user.Psessionid + "%22%7D&clientid=" + user.ClientID + "&psessionid=" + user.Psessionid;

            string html = qwin.POSTHtmlByWininetUTF8(url, postdata);
        }
        /// <summary>
        /// 心跳包
        /// </summary>
        /// <param name="peh">异步委托</param>
        public void Poll2(PollEventHandler peh)
        {
            //线程唯一
            if (PollThread != null) { PollThread.Abort(); }
            //开始心跳
            PollThread = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (!StopPoll)
                    {
                        string postdata = "r=%7B%22clientid%22%3A%22" + user.ClientID + "%22%2C%22psessionid%22%3A%22" + user.Psessionid + "%22%2C%22key%22%3A0%2C%22ids%22%3A%5B%5D%7D&clientid=" + user.ClientID + "&psessionid=" + user.Psessionid;
                        string res = qwin.POSTHtmlByWininetUTF8("http://d.web2.qq.com/channel/poll2", postdata);
                        PollEventHandler pe = peh;
                        pe.BeginInvoke(res, null, null);
                        Tools.sleep(100);
                        
                    }
                    else
                    {
                        Tools.sleep(1000);
                    }
                }
            }));
            PollThread.Start();
        }
        /// <summary>
        /// 心跳异步委托
        /// </summary>
        /// <param name="s">接收返回的心跳数据</param>
        public delegate void PollEventHandler(string s);

        /// <summary>
        /// 退出帐号
        /// </summary>
        public void LogOut()
        {
            //线程唯一
            if (PollThread != null) { PollThread.Abort(); }
            qwin.GetHtmlByWininetUTF8("http://d.web2.qq.com/channel/logout2?ids=&clientid=" + user.ClientID + "&psessionid=" + user.Psessionid);
        }

        #region 私有方法
        /// <summary>
        /// 处理发送消息内容
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private string MakeContent(string con)
        {
            //内容预处理
            con = con.Replace(@"\", @"\\").Replace("\r\n", "\\n");
            //匹配出特殊代码
            MatchCollection rg = Regex.Matches(con, "(\\[bq\\d+\\])");
            //去重复容器
            List<string> ls = new List<string>();
            //开始替换特殊代码
            for (int i = 0; i < rg.Count; i++)
            {
                string txt = rg[i].Groups[1].Value;
                if (!ls.Contains(txt))
                {
                    ls.Add(txt);
                    con = con.Replace(txt, "々" + txt + "々");
                }
            }
            //内容分组
            string[] tmp = Regex.Split(con, "々");
            StringBuilder sb = new StringBuilder();
            foreach (var item in tmp)
            {
                //空内容跳过
                if (item != string.Empty)
                {
                    if (Regex.IsMatch(item, "\\[bq(\\d+)\\]"))
                    {
                        string num = Tools.RegexHelp(item, "\\[bq(\\d+)\\]");
                        sb.Append("[\\\"face\\\"," + num + "],");

                    }
                    else
                    {
                        sb.Append("\\\"" + item + "\\\",");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 计算会话ID
        /// </summary>
        /// <returns></returns>
        private string GetMsgID()
        {
            user.Msg_id = (int.Parse(user.Msg_id) + 1).ToString();
            return user.Msg_id;
        }
        /// <summary>
        /// 取得临时会话sig
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uin"></param>
        /// <returns></returns>
        private string GetGroupSig(string id, string uin)
        {
            string html = qwin.GetHtmlByWininetUTF8("http://d.web2.qq.com/channel/get_c2cmsg_sig2?id=" + id + "&to_uin=" + uin + "&service_type=0&clientid=" + user.ClientID + "&psessionid=" + user.Psessionid);
            return Tools.RegexHelp(html, "value\":\"(.*?)\"");
        }
        
        #region 加密算法
        /// <summary>
        /// 计算UIN
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        private string GetHtmlByWininetUTF8in(string qq)
        {
            string uin = Convert.ToString(Int64.Parse(qq), 16);
            while (uin.Length < 16)
            {
                uin = "0" + uin;
            }
            for (int i = 2; i < uin.Length; i += 4)
            {
                uin = uin.Insert(i, "\\x");
            }
            uin = "\\x" + uin;
            return uin;
        }
        /// <summary>
        /// 计算G_tk
        /// </summary>
        /// <param name="Skey"></param>
        /// <returns></returns>
        private string G_tk(string Skey)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            int hash = 5381;
            for (int i = 0; i < Skey.Length; i++)
            {
                hash += (hash << 5) + (int)asciiEncoding.GetBytes(Skey.Substring(i, 1))[0]; ;
            }
            return Convert.ToString(hash & 0x7fffffff);
        }
        /// <summary>
        /// QQ登录时POST的参数clientid
        /// </summary>
        /// <returns></returns>
        private string GetClientID()
        {
            Random rd = new Random();
            string clientid = rd.Next(10000000, 99999999).ToString();
            return clientid;
        }
        /// <summary>
        /// 取得加密密码
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="pass"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetPass(string qq, string pass, string code)
        {
            string Pass = Md5_32(UnHex(Md5_32(pass) + GetHtmlByWininetUTF8in(qq)) + code);
            return Pass;
        }
        /// <summary>
        /// 取32位MD5|文本型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Md5_32(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string s = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)));
            s = s.Replace("-", string.Empty);
            return s;
        }
        /// <summary>
        /// 取32位MD5|字节集
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        private string Md5_32(byte[] bt)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bt);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 密码加密算法
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private string UnHex(string hex)
        {
            hex = hex.Replace("\\x", string.Empty);
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格
            }
            // 需要将 hex 转换成 byte 数组。 
            byte[] bytes = new byte[hex.Length / 2];
            StringBuilder KnSkyBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                // 每两个字符是一个 byte。 
                //ystem.Globalization.NumberStyles.HexNumber 允许所带的样式
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return Md5_32(bytes);
        }

        /// <summary>
        /// 获取好友列表HASH
        /// </summary>
        /// <param name="qq">QQ号</param>
        /// <param name="ptwebqq">ptwebqq</param>
        /// <returns></returns>
        private string GetHash(string qq, string ptwebqq)
        {
            var a = "";
            string i = "";
            uint uin = uint.Parse(qq);

            a = ptwebqq + "password error";
            while (true)
            {
                if (i.Length <= a.Length)
                {
                    i += uin;
                    if (i.Length == a.Length)
                        break;
                }
                else
                {
                    i = i.Substring(0, a.Length);
                    break;
                }
            }

            int[] E = new int[i.Length];
            for (var c = 0; c < i.Length; c++)
            {
                E[c] = Encoding.UTF8.GetBytes(i)[c] ^ Encoding.UTF8.GetBytes(a)[c];
            }
            string[] a1 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            i = "";
            for (int c = 0; c < E.Length; c++)
            {
                i += a1[E[c] >> 4 & 15];
                i += a1[E[c] & 15];
            }
            return i;
        }
        #endregion

        #endregion
    }
}
