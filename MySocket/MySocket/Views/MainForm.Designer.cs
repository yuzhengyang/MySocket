namespace MySocket.Views
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtClient = new System.Windows.Forms.Button();
            this.BtServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtClient
            // 
            this.BtClient.Location = new System.Drawing.Point(99, 60);
            this.BtClient.Name = "BtClient";
            this.BtClient.Size = new System.Drawing.Size(75, 23);
            this.BtClient.TabIndex = 0;
            this.BtClient.Text = "Client";
            this.BtClient.UseVisualStyleBackColor = true;
            this.BtClient.Click += new System.EventHandler(this.BtClient_Click);
            // 
            // BtServer
            // 
            this.BtServer.Location = new System.Drawing.Point(99, 143);
            this.BtServer.Name = "BtServer";
            this.BtServer.Size = new System.Drawing.Size(75, 23);
            this.BtServer.TabIndex = 1;
            this.BtServer.Text = "Server";
            this.BtServer.UseVisualStyleBackColor = true;
            this.BtServer.Click += new System.EventHandler(this.BtServer_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.BtServer);
            this.Controls.Add(this.BtClient);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtClient;
        private System.Windows.Forms.Button BtServer;
    }
}