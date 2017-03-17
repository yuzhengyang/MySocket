using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winform.Server
{
    public partial class ServerForm : Form
    {
        string Host = "192.168.31.218";
        int Port = 28001;
        Socket Socket;
        Socket s;
        public ServerForm()
        {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
        }

        private void BtConnect_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(Host);
            IPEndPoint ipe = new IPEndPoint(ip, Port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(ipe);
            Socket.Listen(0);
            s = Socket.Accept();
            UITbCmd("已连接");
        }

        private void BtSend_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                while (s.Connected)
                {
                    try
                    {
                        string msg = "Server " + DateTime.Now.ToString("fff");
                        byte[] msgHead = new byte[] { 0x4A, 0x50 };
                        byte[] msgBody = Encoding.GetEncoding("GBK").GetBytes(msg);
                        byte[] msgBodyLength = BitConverter.GetBytes(msgBody.Length);

                        byte[] content = new byte[msgHead.Length + msgBodyLength.Length + msgBody.Length];
                        msgHead.CopyTo(content, 0);
                        msgBodyLength.CopyTo(content, msgHead.Length);
                        msgBody.CopyTo(content, content.Length - msgBody.Length);
                        s.Send(content);

                        UITbSend(msg);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        private void BtReceive_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                List<byte> box = new List<byte>();
                while (s.Connected)
                {
                    try
                    {
                        byte[] recByte = new byte[10];
                        int recLength = s.Receive(recByte, recByte.Length, 0);
                        for (int k = 0; k < recLength; k++)
                        {
                            box.Add(recByte[k]);
                        }

                        if (box.Count > 6 && box[0] == 0x4A && box[1] == 0x50)
                        {
                            int msgBodyLength = BitConverter.ToInt32(new byte[] { box[2], box[3], box[4], box[5] }, 0);
                            if (box.Count >= 6 + msgBodyLength)
                            {
                                byte[] body = box.GetRange(6, msgBodyLength).ToArray();
                                string bodyToGBK = Encoding.GetEncoding("GBK").GetString(body);
                                if (bodyToGBK.Length > 0)
                                {
                                    //收到信息输出
                                    //Console.WriteLine("Receive:" + body.Length + "B,[" + bodyToGBK + "]");
                                    UITbReceive(bodyToGBK);
                                }
                                box.RemoveRange(0, 6 + msgBodyLength);
                            }
                        }
                        else
                        {
                            box.Clear();
                            s.Send(new byte[] { 0 });
                        }
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }
            });
        }

        private void UITbSend(string s)
        {
            BeginInvoke(new Action(() =>
            {
                //发送信息输出
                TbSend.Text += Environment.NewLine;
                TbSend.Text += "S: " + s;
                TbSend.Select(TbSend.Text.Length, 0);
                TbSend.ScrollToCaret();
            }));
        }
        private void UITbReceive(string s)
        {
            BeginInvoke(new Action(() =>
            {
                TbReceive.Text += Environment.NewLine;
                TbReceive.Text += "R: " + s;
                TbReceive.Select(TbReceive.Text.Length, 0);
                TbReceive.ScrollToCaret();
            }));
        }
        private void UITbCmd(string s)
        {
            BeginInvoke(new Action(() =>
            {
                //命令信息输出
                TbCmd.Text += Environment.NewLine;
                TbCmd.Text += "C: " + s;
                TbCmd.Select(TbCmd.Text.Length, 0);
                TbCmd.ScrollToCaret();
            }));
        }
    }
}
