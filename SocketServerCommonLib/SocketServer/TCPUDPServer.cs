/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 网站： CSkin界面库 http://www.cskin.net 作者:乔克斯 感谢免费支持
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/
using CCWin;
using CCWin.SkinControl;
using System;
using System.Windows.Forms;
using SocketServerCommonLib;
using System.Threading;
using System.Drawing;
using System.IO;

namespace SocketServer
{
    public partial class TCPUDPServer : CCSkinMain
    {
        AsyncSocketServer TcpServer;

        AsyncUDPServer UdpServer;

        /// <summary>
        /// 用户连接多少次
        /// </summary>
        int TCPUserCount = 0;
        int TCPDeviceCount = 0;
        public TCPUDPServer()
        {
            InitializeComponent();
            DelegateState.ServerStateInfo = ServerShowStateInfo;
            DelegateState.TeartbeatServerStateInfo = TeartbeatShowStateInfo;
            DelegateState.AddTCPuserStateInfo = AddTCPuser;
            DelegateState.AddTCPdeviceStateInfo = AddTCPdevice;
            DelegateState.ReomveTCPStateInfo = ReomveTCP;
            DelegateState.ServerConnStateInfo = ConnStateInfo;
        }

        #region  AmosLi produce <启动服务模块>

        /// <summary>
        /// 启动TCP服务
        /// </summary>
        private void btnTCP_Click(object sender, EventArgs e)
        {
            btnTCP.Enabled = false;

            if (TcpServer == null)
                TcpServer = new AsyncSocketServer();
            if (!TcpServer.IsStartListening)
            {
                TcpServer.Start();
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "TCP服务器启动" + Environment.NewLine);
                lblTCP.Text = "TCP服务器地址:" + TcpServer.serverconfig.ListenIp + ":" + TcpServer.serverconfig.ListenPort;
                PicBoxTCP.BackgroundImage = Properties.Resources._07822_48x48x8BPP_;
                btnTCP.Text = "TCP停止服务";
            }
            else
            {
                TcpServer.Stop();
                PicBoxTCP.BackgroundImage = Properties.Resources._07821_48x48x8BPP_;
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "TCP服务器停止" + Environment.NewLine);
            }
            btnTCP.Enabled = true;
        }


        /// <summary>
        /// UDP服务
        /// </summary>
        private void btnUDP_Click(object sender, EventArgs e)
        {
            btnUDP.Enabled = false;
            if (UdpServer == null)
                UdpServer = new AsyncUDPServer();

            if (!UdpServer.IsStartListening)
            {
                UdpServer.Start();
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "UDP服务器启动" + Environment.NewLine);
                lblUDP.Text = "UDP服务器地址:" + HelpCommonLib.NetworkAddress.GetIPAddress() + ":" + UdpServer.ListenProt;
                PicBoxUDP.BackgroundImage = Properties.Resources._07822_48x48x8BPP_;
                btnUDP.Text = "UDP停止服务";
                btnUDP.Enabled = true;
            }
            else
            {
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "UDP服务器停止" + Environment.NewLine);
                UdpServer.Close();
                PicBoxUDP.BackgroundImage = Properties.Resources._07821_48x48x8BPP_;
                btnUDP.Text = "UDP端口错误";
            }
        }
        #region

        #region TCP回调函数操作
        void AddTCPuser(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                TCPUserCount++;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = TCPUserCount.ToString();
                lvi.SubItems.Add(userToken.ConnectSocket.RemoteEndPoint.ToString());
                lvi.SubItems.Add(userToken.UserName);
                lvi.SubItems.Add(userToken.ConnectDateTime.ToString());
                lvi.SubItems.Add("TCP");
                tpe3list1.Items.Add(lvi);
            }));
        }

        /// <summary>
        /// 删除用户或者设备
        /// </summary>
        /// <param name="userToken"></param>
        void ReomveTCP(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                if (userToken.isDevice)
                {
                    for (int i = 0; i < tpe2list1.Items.Count; i++)
                    {
                        if (userToken.UserName == tpe2list1.Items[i].SubItems[2].Text)
                        {
                            tpe2list1.Items.Remove(tpe2list1.Items[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tpe3list1.Items.Count; i++)
                    {
                        if (userToken.UserName == tpe3list1.Items[i].SubItems[2].Text)
                        {
                            tpe3list1.Items.Remove(tpe3list1.Items[i]);
                        }
                    }
                }
            }));
        }

        void AddTCPdevice(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                TCPDeviceCount++;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = TCPDeviceCount.ToString();
                lvi.SubItems.Add(userToken.ConnectSocket.RemoteEndPoint.ToString());
                lvi.SubItems.Add(userToken.UserName);
                lvi.SubItems.Add(userToken.ConnectDateTime.ToString());
                lvi.SubItems.Add("TCP");
                tpe2list1.Items.Add(lvi);
            }));
        }
        #endregion

        void ConnStateInfo(string RemoteIp, string TCPUDP)
        {
            this.Invoke(new ThreadStart(delegate
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = listAllView.Items.Count.ToString();
                lvi.SubItems.Add(RemoteIp);
                lvi.SubItems.Add(DateTime.Now.ToString());
                lvi.SubItems.Add(TCPUDP);
                listAllView.Items.Add(lvi);
            }));
        }


        /// <summary>
        /// 信息添加
        /// </summary>
        /// <param name="msg"></param>
        void ServerShowStateInfo(string msg)
        {
            this.Invoke(new ThreadStart(delegate
            {
                tpe2txtMsg.AppendText(DateTime.Now + ":" + msg + Environment.NewLine);
            }));
        }

        /// <summary>
        /// 心跳时间
        /// </summary>
        void TeartbeatShowStateInfo(int num)
        {
            this.Invoke(new ThreadStart(delegate
            {
                txtMsg.AppendText(DateTime.Now + ":" + num + "连接检测");
                lblNum1.NormlBack = ImageListAllUpdate(num / 10 % 10);
                lblNum2.NormlBack = ImageListAllUpdate(num % 10);
            }));
        }
        #endregion
        /// <summary>
        /// 图片更换
        /// </summary>
        Image ImageListAllUpdate(int Num)
        {
            switch (Num)
            {
                case 0:
                    return Properties.Resources._00034_17x25x8BPP_;
                case 1:
                    return Properties.Resources._00035_17x25x8BPP_;
                case 2:
                    return Properties.Resources._00036_17x25x8BPP_;
                case 3:
                    return Properties.Resources._00037_17x25x8BPP_;
                case 4:
                    return Properties.Resources._00038_17x25x8BPP_;
                case 5:
                    return Properties.Resources._00039_17x25x8BPP_;
                case 6:
                    return Properties.Resources._00040_17x25x8BPP_;
                case 7:
                    return Properties.Resources._00041_17x25x8BPP_;
                case 8:
                    return Properties.Resources._00042_17x25x8BPP_;
                case 9:
                    return Properties.Resources._00043_17x25x8BPP_;
                default:
                    return null;
            }
        }
        #endregion
        /// <summary>
        /// 刷新设备列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkdeviceRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TcpServer == null)
                return;

            linkdeviceRefresh.Enabled = false;
            tpe2list1.Items.Clear();
            SocketUserToken[] userTokenArray = null;
            TcpServer.AsyncSocketDeviceList.CopyList(ref userTokenArray);
            int num = 0;
            for (int i = 0; i < userTokenArray.Length; i++)
            {
                if (userTokenArray[i].LoginFlag)
                {
                    AddTCPdevice(userTokenArray[i]);
                }
                num++;
            }
            lbldevice.Text = "上次刷新是在 " + DateTime.Now.Hour + " 点，共连接 " + userTokenArray.Length + " 个客户。";
            linkdeviceRefresh.Enabled = true;
        }

        private void linkuserRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TcpServer == null)
                return;

            linkuserRefresh.Enabled = false;
            tpe3list1.Items.Clear();
            SocketUserToken[] userTokenArray = null;
            TcpServer.AsyncSocketUserList.CopyList(ref userTokenArray);
            int num = 0;
            for (int i = 0; i < userTokenArray.Length; i++)
            {
                if (userTokenArray[i].LoginFlag)
                {
                    AddTCPuser(userTokenArray[i]);
                    num++;
                }
            }
            lbluser.Text = "上次刷新是在 " + DateTime.Now.Hour + " 点，共连接 " + num + " 个客户。";
            linkuserRefresh.Enabled = true;
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        private void linkSaveMsg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter write = new StreamWriter(saveFileDialog1.FileName))
                {
                    write.WriteLine(tpe2txtMsg.Text);
                }
            }
        }
    }
}
