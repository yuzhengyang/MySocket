using MySocket.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySocket.Views
{
    public partial class ServerForm : Form
    {
        SocketHelper socketHelper;
        public ServerForm()
        {
            InitializeComponent();
        }

        private void BtReceive_Click(object sender, EventArgs e)
        {
            if (socketHelper == null)
            {
                socketHelper = new SocketHelper("192.168.31.238", 28001);
                socketHelper.Init();
                socketHelper.Receive();
                socketHelper.ReceiveByteContent += ReceiveMsg;
            }
            else
            {
                socketHelper.Dispose();
            }
        }

        public void ReceiveMsg(byte[] msg)
        {
            string bodyToGBK = Encoding.GetEncoding("GBK").GetString(msg);
            BeginInvoke(new Action(() => { TbTxt.AppendText(bodyToGBK + Environment.NewLine); }));
        }
    }
}
