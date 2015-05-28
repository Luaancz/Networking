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
					// Simple connect as always. We're going to keep the callbacks on the UI thread this time, though - every await is followed by some UI operation anyway.
					await client.ConnectAsync("localhost", 31224);

					// We're connected now, so allow disconnects.
					btnDisconnect.Enabled = true;

					cancellationTokenSource = new CancellationTokenSource();
					stream = client.GetStream();

					// We'll send the login request. If the server disconnects us, we'll know by reading the next message from the network stream.
					await stream.SendStringMessage(MessageId.LoginRequest, tbxName.Text);

					var cancellationToken = cancellationTokenSource.Token;
					var cancellationTask = cancellationToken.AsTask();

					grpChat.Enabled = true;
					while (!cancellationToken.IsCancellationRequested)
					{
						// Start asynchronous I/O request for the next message from server
						var messageTask = stream.ReadRawMessage();

						// We'll wait for either a new message, or a cancellation - this allows us to close the connection when the disconnect button is clicked.
						await Task.WhenAny(messageTask, cancellationTask);

						// Do we have a message?
						if (messageTask.IsCompleted)
						{
							// Get the actual message from the task, or an exception. This would be the place to handle the exception.
							byte[] message = messageTask.GetAwaiter().GetResult();

							// Oh, the server closed the connection. What can you do :)
							if (message == null)
							{
								tbxChat.AppendText(string.Format("Disconnected by server.{0}", Environment.NewLine));

								return;
							}

							// Let's find out what message it is...
							switch ((MessageId)message[0])
							{
								case MessageId.ChatMessage:
									{
										// Just write the chat messages to the chat window.
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

						// Did we click Disconnect?
						if (cancellationTask.IsCompleted)
						{
							tbxChat.AppendText(string.Format("Disconnecting{0}", Environment.NewLine));

							return;
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

				// Only send if we have a connection open
				if (btnDisconnect.Enabled)
				{
					var messageBytes = Encoding.UTF8.GetByteCount(tbxMessage.Text);

					var data = new byte[messageBytes + 1];
					data[0] = (byte)MessageId.ChatMessage;
					Encoding.UTF8.GetBytes(tbxMessage.Text, 0, tbxMessage.Text.Length, data, 1);

					// Empty the message textbox - this allows the user to start typing the next message while waiting for the one being sent.
					// This way is a bit tricky when doing proper error handling - what if SendRawMessage fails?
					tbxMessage.Text = string.Empty;

					await stream.SendRawMessage(data);
				}
			}
			finally
			{
				btnSend.Enabled = true;
			}
		}

		private void btnDisconnect_Click(object sender, EventArgs e)
		{
			// Signal the connection handler to close the connection
			if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
		}
	}
}
