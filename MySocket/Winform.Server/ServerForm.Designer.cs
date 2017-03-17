namespace Winform.Server
{
    partial class ServerForm
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
            this.TbCmd = new System.Windows.Forms.TextBox();
            this.TbSend = new System.Windows.Forms.TextBox();
            this.TbReceive = new System.Windows.Forms.TextBox();
            this.BtReceive = new System.Windows.Forms.Button();
            this.BtSend = new System.Windows.Forms.Button();
            this.BtConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TbCmd
            // 
            this.TbCmd.Location = new System.Drawing.Point(346, 12);
            this.TbCmd.Multiline = true;
            this.TbCmd.Name = "TbCmd";
            this.TbCmd.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TbCmd.Size = new System.Drawing.Size(161, 262);
            this.TbCmd.TabIndex = 5;
            // 
            // TbSend
            // 
            this.TbSend.Location = new System.Drawing.Point(179, 12);
            this.TbSend.Multiline = true;
            this.TbSend.Name = "TbSend";
            this.TbSend.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TbSend.Size = new System.Drawing.Size(161, 262);
            this.TbSend.TabIndex = 4;
            // 
            // TbReceive
            // 
            this.TbReceive.Location = new System.Drawing.Point(12, 12);
            this.TbReceive.Multiline = true;
            this.TbReceive.Name = "TbReceive";
            this.TbReceive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TbReceive.Size = new System.Drawing.Size(161, 262);
            this.TbReceive.TabIndex = 3;
            // 
            // BtReceive
            // 
            this.BtReceive.Location = new System.Drawing.Point(432, 317);
            this.BtReceive.Name = "BtReceive";
            this.BtReceive.Size = new System.Drawing.Size(75, 23);
            this.BtReceive.TabIndex = 8;
            this.BtReceive.Text = "Receive";
            this.BtReceive.UseVisualStyleBackColor = true;
            this.BtReceive.Click += new System.EventHandler(this.BtReceive_Click);
            // 
            // BtSend
            // 
            this.BtSend.Location = new System.Drawing.Point(351, 317);
            this.BtSend.Name = "BtSend";
            this.BtSend.Size = new System.Drawing.Size(75, 23);
            this.BtSend.TabIndex = 7;
            this.BtSend.Text = "Send";
            this.BtSend.UseVisualStyleBackColor = true;
            this.BtSend.Click += new System.EventHandler(this.BtSend_Click);
            // 
            // BtConnect
            // 
            this.BtConnect.Location = new System.Drawing.Point(270, 317);
            this.BtConnect.Name = "BtConnect";
            this.BtConnect.Size = new System.Drawing.Size(75, 23);
            this.BtConnect.TabIndex = 6;
            this.BtConnect.Text = "Connect";
            this.BtConnect.UseVisualStyleBackColor = true;
            this.BtConnect.Click += new System.EventHandler(this.BtConnect_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 352);
            this.Controls.Add(this.BtReceive);
            this.Controls.Add(this.BtSend);
            this.Controls.Add(this.BtConnect);
            this.Controls.Add(this.TbCmd);
            this.Controls.Add(this.TbSend);
            this.Controls.Add(this.TbReceive);
            this.Name = "ServerForm";
            this.Text = "ServerForm";
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TbCmd;
        private System.Windows.Forms.TextBox TbSend;
        private System.Windows.Forms.TextBox TbReceive;
        private System.Windows.Forms.Button BtReceive;
        private System.Windows.Forms.Button BtSend;
        private System.Windows.Forms.Button BtConnect;
    }
}