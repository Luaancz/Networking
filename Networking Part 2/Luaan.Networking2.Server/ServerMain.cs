using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Luaan.Networking2.Messages;

namespace Luaan.Networking2.Server
{
	public class ServerMain : ServerBase
	{
		private readonly ConcurrentDictionary<string, LoginInformation> users;

		public ServerMain()
		{
			this.users = new ConcurrentDictionary<string, LoginInformation>();
			Broadcast += _ => {};
		}

		private event Action<byte[]> Broadcast;

		void BroadcastChatMessage(string message)
		{
			Console.WriteLine(message);

			var messageByteCount = Encoding.UTF8.GetByteCount(message);
			var data = new byte[messageByteCount + 1];

			data[0] = (byte)MessageId.ChatMessage;
			Encoding.UTF8.GetBytes(message, 0, message.Length, data, 1);

			Broadcast(data);
		}

		protected override async Task Accept(TcpClient client)
		{
			try
			{
				var stream = client.GetStream();

				var login = default(LoginInformation);

				// Note that local variables are effectively local with respect to any individual connection. Pretty handy.
				try
				{
					login = await HandleLogin(stream);

					if (login == null)
					{
						client.Close();
						return;
					}

					CancellationToken.ThrowIfCancellationRequested();

					await HandleChat(stream, login);
				}
				finally
				{
					if (login != null)
					{
						this.users.TryRemove(login.Name, out login);
						BroadcastChatMessage(string.Format("{0} left us.", login.Name));
					}
				}
			}
			finally
			{
				client.Close();
			}
		}

		async Task<LoginInformation> HandleLogin(Stream stream)
		{
			var message = await stream.ReadRawMessage();

			if (message == null) return null;

			// Check the message ID
			if (message[0] != (byte)MessageId.LoginRequest) return null;

			// The rest of the message is the name
			var name = Encoding.UTF8.GetString(message, 1, message.Length - 1);

			var login = new LoginInformation(name);
			var index = 1;

			// Let's make sure we don't have another user with the same name
			while (!this.users.TryAdd(login.Name, login))
			{
				login = new LoginInformation(name + index.ToString(CultureInfo.InvariantCulture));
			}

			BroadcastChatMessage(string.Format("{0} joined us!", login.Name));

			return login;
		}

		async Task HandleChat(Stream stream, LoginInformation login)
		{
			var queuedMessages = new BufferBlock<byte[]>();
			
			Action<byte[]> broadcastHandler =
				message =>
				{
					queuedMessages.Post(message);
				};

			try
			{
				Broadcast += broadcastHandler;

				Task<byte[]> readTask = stream.ReadRawMessage();
				Task<byte[]> broadcastTask = queuedMessages.ReceiveAsync(CancellationToken);
				
				while (!CancellationToken.IsCancellationRequested)
				{
					await Task.WhenAny(readTask, broadcastTask);

					if (readTask.IsCompleted)
					{
						var message = readTask.GetAwaiter().GetResult();

						if (message == null) return;

						switch ((MessageId)message[0])
						{
							case MessageId.ChatMessage:
								{
									BroadcastChatMessage(string.Format("{0}> {1}", login.Name, Encoding.UTF8.GetString(message, 1, message.Length - 1)));

									break;
								}
							default:
								{
									Console.WriteLine("Unexpected message 0x{0}.", message[0].ToString("h2"));

									break;
								}
						}

						readTask = stream.ReadRawMessage();
					}

					if (broadcastTask.IsCompleted)
					{
						var broadcastMessage = broadcastTask.GetAwaiter().GetResult();

						await stream.SendRawMessage(broadcastMessage);

						broadcastTask = queuedMessages.ReceiveAsync(CancellationToken);
					}
				}
			}
			finally
			{
				Broadcast -= broadcastHandler;
			}
		}
	}
}
