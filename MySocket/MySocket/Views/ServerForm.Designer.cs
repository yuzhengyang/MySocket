namespace MySocket.Views
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
            this.TbTxt = new System.Windows.Forms.TextBox();
            this.BtReceive = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TbTxt
            // 
            this.TbTxt.Location = new System.Drawing.Point(13, 41);
            this.TbTxt.Multiline = true;
            this.TbTxt.Name = "TbTxt";
            this.TbTxt.Size = new System.Drawing.Size(894, 733);
            this.TbTxt.TabIndex = 0;
            // 
            // BtReceive
            // 
            this.BtReceive.Location = new System.Drawing.Point(832, 12);
            this.BtReceive.Name = "BtReceive";
            this.BtReceive.Size = new System.Drawing.Size(75, 23);
            this.BtReceive.TabIndex = 1;
            this.BtReceive.Text = "Receive";
            this.BtReceive.UseVisualStyleBackColor = true;
            this.BtReceive.Click += new System.EventHandler(this.BtReceive_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 786);
            this.Controls.Add(this.BtReceive);
            this.Controls.Add(this.TbTxt);
            this.Name = "ServerForm";
            this.Text = "ServerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TbTxt;
        private System.Windows.Forms.Button BtReceive;
    }
}