using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luaan.Networking2.Server
{
	public abstract class ServerBase
	{
		private TcpListener listener;
		private CancellationTokenSource cts;
		private CancellationToken token;

		protected CancellationToken CancellationToken { get { return this.token; } }

		public void Start()
		{
			cts = new CancellationTokenSource();
			token = cts.Token;

			listener = new TcpListener(IPAddress.Any, 31224);
			listener.Start();

			// Note that we're not awaiting here - this is going to return almost immediately. 
			// We're storing the task in a variable to make it explicit that this is not a case of forgotten await :)
			var _ = Listen();
		}

		async Task Listen()
		{
			var client = default(TcpClient);

			while (!token.IsCancellationRequested)
			{
				try
				{
					client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
				}
				catch (ObjectDisposedException)
				{
					// The listener has been stopped.
					return;
				}

				if (client == null) return;

				// Again, there's no await - the Accept handler is going to return immediately so that we can handle the next client.
				var _ = Accept(client);
			}
		}

		protected abstract Task Accept(TcpClient client);

		public void Stop()
		{
			cts.Cancel();
			listener.Stop();
		}
	}
}
