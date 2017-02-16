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

namespace WinFormTest.SocketClient
{
    public partial class Form1 : Form
    {
        Dictionary<int, Socket> Sockets = new Dictionary<int, Socket>();
        Dictionary<int, Button> Buttons = new Dictionary<int, Button>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void OpenFire(int index)
        {
            if (Sockets[index] == null)
            {
                try
                {
                    //读取host、port和发送间隔时间
                    string host = TbIP.Text;
                    int port = int.Parse(TbPort.Text);
                    int interval = int.Parse(TbInterval.Text);
                    string msg = TbMsg.Text;
                    //创建socket对象并连接
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    s.Connect(host, port);
                    //连接成功为dictionary赋值
                    Sockets[index] = s;
                    BeginInvoke(new Action(() =>
                    {
                        Buttons[index].BackColor = Color.Green;
                    }));
                    //启动发送线程
                    while (Sockets[index].Connected)
                    {
                        if (Sockets[index].Connected)
                        {
                            try
                            {
                                byte[] msgHead = new byte[] { 255, 254 };
                                byte[] msgBody = Encoding.GetEncoding("GBK").GetBytes(msg);
                                byte[] msgBodyLength = BitConverter.GetBytes(msgBody.Length);

                                byte[] content = new byte[msgHead.Length + msgBodyLength.Length + msgBody.Length];
                                msgHead.CopyTo(content, 0);
                                msgBodyLength.CopyTo(content, msgHead.Length);
                                msgBody.CopyTo(content, content.Length - msgBody.Length);
                                s.Send(content);
                            }
                            catch (Exception e)
                            {
                                CeaseFire(index);
                                break;
                            }
                        }
                        Thread.Sleep(interval);
                    }
                    //启动接受线程
                    //string recStr = "";
                    //byte[] recBytes = new byte[4096];
                    //int bytes = s.Receive(recBytes, recBytes.Length, 0);
                    //recStr += Encoding.ASCII.GetString(recBytes, 0, bytes);
                }
                catch (Exception e) { }
            }
        }
        private void CeaseFire(int index)
        {
            if (Sockets[index] != null)
            {
                Sockets[index].Shutdown(SocketShutdown.Both);
                Sockets[index].Close();
                Sockets[index].Dispose();
                Sockets[index] = null;
                BeginInvoke(new Action(() =>
                {
                    Buttons[index].BackColor = Color.White;
                }));
            }
        }
        private void PiuPiuPiu(int index, Button button)
        {
            //如果不包含index（第一次初始化）
            if (!Sockets.ContainsKey(index))
            {
                Buttons.Add(index, button);
                Sockets.Add(index, null);
            }
            //如果index的项is null（初始化完毕/释放过资源）
            if (Sockets[index] == null)
            {
                Task.Factory.StartNew(() =>
                {
                    OpenFire(index);
                });
            }
            //如果index的项not null（正在执行）
            if (Sockets[index] != null)
            {
                Task.Factory.StartNew(() =>
                {
                    CeaseFire(index);
                });
            }
        }
        #region Button Click 事件
        private void button1_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(1, (Button)sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(2, (Button)sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(3, (Button)sender);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(4, (Button)sender);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(5, (Button)sender);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(6, (Button)sender);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(7, (Button)sender);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(8, (Button)sender);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(9, (Button)sender);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            PiuPiuPiu(10, (Button)sender);
        }
        #endregion
    }
}
