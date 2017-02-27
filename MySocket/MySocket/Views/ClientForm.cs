using MySocket.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySocket.Views
{
    public partial class ClientForm : Form
    {
        Dictionary<int, SocketHelper> Sockets = new Dictionary<int, SocketHelper>();
        Dictionary<int, Button> Buttons = new Dictionary<int, Button>();
        public ClientForm()
        {
            InitializeComponent();
        }
        private void ClientForm_Load(object sender, EventArgs e)
        {

        }
        private void OpenFire(int index)
        {
            if (Sockets[index] == null)
            {
                try
                {
                    //读取host、port、发送间隔、发送信息
                    string host = TbIP.Text;
                    int port = int.Parse(TbPort.Text);
                    int interval = int.Parse(TbInterval.Text);
                    string msg = TbMsg.Text;
                    //初始化SocketHelper
                    SocketHelper sh = new SocketHelper(host, port);
                    sh.Init();
                    sh.Connect();
                    Sockets[index] = sh;
                    BeginInvoke(new Action(() =>
                    {
                        Buttons[index].BackColor = Color.Green;
                    }));
                    //启动发送线程
                    while (Sockets[index].IsConnected)
                    {
                        try
                        {
                            Sockets[index].Send(msg);
                        }
                        catch (Exception e)
                        {
                            CeaseFire(index);
                            break;
                        }
                        Thread.Sleep(interval);
                    }
                }
                catch (Exception e) { }
            }
        }
        private void CeaseFire(int index)
        {
            if (Sockets[index] != null)
            {
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
