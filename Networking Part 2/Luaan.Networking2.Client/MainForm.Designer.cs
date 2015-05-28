namespace Luaan.Networking2.Client
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
			this.grpConnect = new System.Windows.Forms.GroupBox();
			this.tbxName = new System.Windows.Forms.TextBox();
			this.lblName = new System.Windows.Forms.Label();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.grpChat = new System.Windows.Forms.GroupBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.tbxMessage = new System.Windows.Forms.TextBox();
			this.tbxChat = new System.Windows.Forms.TextBox();
			this.grpConnect.SuspendLayout();
			this.grpChat.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpConnect
			// 
			this.grpConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpConnect.Controls.Add(this.tbxName);
			this.grpConnect.Controls.Add(this.lblName);
			this.grpConnect.Controls.Add(this.btnDisconnect);
			this.grpConnect.Controls.Add(this.btnConnect);
			this.grpConnect.Location = new System.Drawing.Point(12, 12);
			this.grpConnect.Name = "grpConnect";
			this.grpConnect.Size = new System.Drawing.Size(394, 39);
			this.grpConnect.TabIndex = 0;
			this.grpConnect.TabStop = false;
			this.grpConnect.Text = "Connect";
			// 
			// tbxName
			// 
			this.tbxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxName.Location = new System.Drawing.Point(50, 13);
			this.tbxName.Name = "tbxName";
			this.tbxName.Size = new System.Drawing.Size(176, 20);
			this.tbxName.TabIndex = 1;
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(6, 16);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(38, 13);
			this.lblName.TabIndex = 1;
			this.lblName.Text = "Name:";
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDisconnect.Enabled = false;
			this.btnDisconnect.Location = new System.Drawing.Point(313, 11);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
			this.btnDisconnect.TabIndex = 3;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
			// 
			// btnConnect
			// 
			this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnConnect.Location = new System.Drawing.Point(232, 11);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(75, 23);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// grpChat
			// 
			this.grpChat.Controls.Add(this.btnSend);
			this.grpChat.Controls.Add(this.tbxMessage);
			this.grpChat.Controls.Add(this.tbxChat);
			this.grpChat.Enabled = false;
			this.grpChat.Location = new System.Drawing.Point(12, 57);
			this.grpChat.Name = "grpChat";
			this.grpChat.Size = new System.Drawing.Size(394, 209);
			this.grpChat.TabIndex = 1;
			this.grpChat.TabStop = false;
			this.grpChat.Text = "Chat";
			// 
			// btnSend
			// 
			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.Location = new System.Drawing.Point(313, 180);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 23);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// tbxMessage
			// 
			this.tbxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxMessage.Location = new System.Drawing.Point(9, 183);
			this.tbxMessage.Name = "tbxMessage";
			this.tbxMessage.Size = new System.Drawing.Size(298, 20);
			this.tbxMessage.TabIndex = 1;
			// 
			// tbxChat
			// 
			this.tbxChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbxChat.Location = new System.Drawing.Point(9, 19);
			this.tbxChat.Multiline = true;
			this.tbxChat.Name = "tbxChat";
			this.tbxChat.ReadOnly = true;
			this.tbxChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbxChat.Size = new System.Drawing.Size(379, 155);
			this.tbxChat.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(418, 278);
			this.Controls.Add(this.grpChat);
			this.Controls.Add(this.grpConnect);
			this.Name = "MainForm";
			this.Text = "Chat client";
			this.grpConnect.ResumeLayout(false);
			this.grpConnect.PerformLayout();
			this.grpChat.ResumeLayout(false);
			this.grpChat.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grpConnect;
		private System.Windows.Forms.TextBox tbxName;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Button btnDisconnect;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.GroupBox grpChat;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.TextBox tbxMessage;
		private System.Windows.Forms.TextBox tbxChat;
	}
}

