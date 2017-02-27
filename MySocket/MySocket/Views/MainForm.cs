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
    public partial class MainForm : Form
    {
        ClientForm clientForm;
        ServerForm serverForm;
        public MainForm()
        {
            InitializeComponent();
        }

        private void BtClient_Click(object sender, EventArgs e)
        {
            if (clientForm != null && !clientForm.IsDisposed)
            {
                clientForm.Show();
            }
            else
            {
                clientForm = new ClientForm();
                clientForm.Show();
            }
        }

        private void BtServer_Click(object sender, EventArgs e)
        {
            if (serverForm != null && !serverForm.IsDisposed)
            {
                serverForm.Show();
            }
            else
            {
                serverForm = new ServerForm();
                serverForm.Show();
            }
        }
    }
}
