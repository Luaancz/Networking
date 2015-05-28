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

			// This allows us to safely invoke Broadcast. Since we assume that there's going to be multiple clients, this is a good trade-off.
			Broadcast += _ => {};
		}

		private event Action<byte[]> Broadcast;

		/// <summary>
		/// Broadcasts the specified chat message to all connected clients.
		/// </summary>
		/// <param name="message">The message text.</param>
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

				// Note that local variables are effectively local with respect to any individual connection. Pretty handy - we get a state machine for free.
				try
				{
					// Start the login process
					login = await HandleLogin(stream);

					if (login == null) return;

					CancellationToken.ThrowIfCancellationRequested();

					// Start the main chat message loop
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
			// Buffer block allows us to easily post messages from our broadcast event handler to our main loop
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
					// We'll wait for either data from the client, or a broadcast message from the server
					await Task.WhenAny(readTask, broadcastTask);

					// Did we read some data from the client?
					if (readTask.IsCompleted)
					{
						// Get the message. If there was an exception during the task, it will be rethrown here.
						var message = readTask.GetAwaiter().GetResult();

						// The client disconnected - we're done.
						if (message == null) return;

						// What kind of message did we receive?
						switch ((MessageId)message[0])
						{
							case MessageId.ChatMessage:
								{
									// Broadcast all incoming messages to everyone
									BroadcastChatMessage(string.Format("{0}> {1}", login.Name, Encoding.UTF8.GetString(message, 1, message.Length - 1)));

									break;
								}
							default:
								{
									Console.WriteLine("Unexpected message 0x{0}.", message[0].ToString("h2"));

									break;
								}
						}

						// Start another asynchronous I/O request
						readTask = stream.ReadRawMessage();
					}

					// Do we have any broadcast messages to send?
					if (broadcastTask.IsCompleted)
					{
						// Get the message. If there was an exception during the task, it will be rethrown here.
						var broadcastMessage = broadcastTask.GetAwaiter().GetResult();

						// The message is ready to be sent as-is, so just go ahead
						await stream.SendRawMessage(broadcastMessage);

						// Start another wait for broadcast messages
						broadcastTask = queuedMessages.ReceiveAsync(CancellationToken);
					}
				}
			}
			finally
			{
				// We have to unregister the broadcast message handler. This is quite important - global event handlers are a great way to introduce memory leaks :)
				Broadcast -= broadcastHandler;
			}
		}
	}
}
