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
using Luaan.Networking2.Messages;

namespace Luaan.Networking2.Client
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		NetworkStream stream;
		CancellationTokenSource cancellationTokenSource;

		private async void btnConnect_Click(object sender, EventArgs e)
		{
			try
			{
				btnConnect.Enabled = false;
				tbxName.Enabled = false;

				var client = new TcpClient();

				try
				{
					await client.ConnectAsync("localhost", 31224);

					btnDisconnect.Enabled = true;

					cancellationTokenSource = new CancellationTokenSource();

					stream = client.GetStream();

					await stream.SendStringMessage(MessageId.LoginRequest, tbxName.Text);

					var cancellationToken = cancellationTokenSource.Token;

					var cancellationTask = cancellationToken.AsTask();
					grpChat.Enabled = true;
					while (!cancellationToken.IsCancellationRequested)
					{
						var messageTask = stream.ReadRawMessage();

						await Task.WhenAny(messageTask, cancellationTask);

						if (messageTask.IsCompleted)
						{
							byte[] message = messageTask.GetAwaiter().GetResult();

							if (message == null)
							{
								tbxChat.AppendText(string.Format("Disconnected by server.{0}", Environment.NewLine));

								return;
							}

							switch ((MessageId)message[0])
							{
								case MessageId.ChatMessage:
									{
										tbxChat.AppendText(Encoding.UTF8.GetString(message, 1, message.Length - 1) + Environment.NewLine);

										break;
									}
								default:
									{
										tbxChat.AppendText(string.Format("Unexpected message 0x{0:h2}{1}", message[0], Environment.NewLine));

										break;
									}
							}
						}

						if (cancellationTask.IsCompleted)
						{
							tbxChat.AppendText(string.Format("Disconnecting{0}", Environment.NewLine));

							break;
						}
					}
				}
				finally
				{

					client.Close();
				}
			}
			finally
			{
				btnConnect.Enabled = true;
				btnDisconnect.Enabled = false;
				grpChat.Enabled = false;
				tbxName.Enabled = true;
			}
		}

		private async void btnSend_Click(object sender, EventArgs e)
		{
			try
			{
				btnSend.Enabled = false;

				if (btnDisconnect.Enabled)
				{
					var messageBytes = Encoding.UTF8.GetByteCount(tbxMessage.Text);

					var data = new byte[messageBytes + 1];
					data[0] = (byte)MessageId.ChatMessage;
					Encoding.UTF8.GetBytes(tbxMessage.Text, 0, tbxMessage.Text.Length, data, 1);

					try
					{
						tbxMessage.Enabled = false;
						await stream.SendRawMessage(data);
					}
					finally
					{
						tbxMessage.Enabled = true;
					}

					tbxMessage.Text = string.Empty;
				}
			}
			finally
			{
				btnSend.Enabled = true;
			}
		}

		private void btnDisconnect_Click(object sender, EventArgs e)
		{
			if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
		}
	}
}
