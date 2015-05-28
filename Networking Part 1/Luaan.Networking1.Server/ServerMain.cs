using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luaan.Networking1.Server
{
    public class ServerMain
    {
        private TcpListener listener;
        private CancellationTokenSource cts;
        private CancellationToken token;

        public void Start()
        {
            cts = new CancellationTokenSource();
            token = cts.Token;

            listener = new TcpListener(IPAddress.Any, 31224);
            listener.Start();

			// Note that we're not awaiting here - this is going to return almost immediately. 
			// We're storing the task in a variable to make it explicit that this is not a case of forgotten await :)
            var t = Listen();
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
                var t = Accept(client);
            }
        }

        async Task Accept(TcpClient client)
        {
			// The using makes sure we're going to dispose of the client. This is very easy thanks to await :)
			using (client)
			{
				byte[] buffer = new byte[512];
				var bytesRead = 0;
				var stream = client.GetStream();

				// First, we need to know how much data to read. We've got a 4-byte fixed-size header to handle that.
				// It's unlikely we'd read the header in multiple ReadAsync calls (it's only 4 bytes :)), but it's good practice anyway.
				var headerRead = 0;
				while (headerRead < 4 && (bytesRead = await stream.ReadAsync(buffer, headerRead, 4 - headerRead).ConfigureAwait(false)) > 0)
				{
					headerRead += bytesRead;
				}

				if (headerRead < 4) return; // We failed to read the header.

				var bytesRemaining = BitConverter.ToInt32(buffer, 0);
				Console.WriteLine("Receiving {0} bytes.", bytesRemaining);

				// Now we know how much we have to read, so let's read everything and write it back out.
				while (bytesRemaining > 0 && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
				{
					await stream.WriteAsync(buffer, 0, bytesRead);

					bytesRemaining -= bytesRead;
				}

				// If ReadAsync returns zero, it means the connection was closed from the other side. If it doesn't, we have to close it ourselves.
				if (bytesRead != 0) client.Close(); // Do a graceful shutdown
			}
        }

        public void Stop()
        {
            cts.Cancel();
            listener.Stop();
        }
    }
}
