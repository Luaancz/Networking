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

            Listen();
        }

        async void Listen()
        {
            var client = default(TcpClient);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    client = await listener.AcceptTcpClientAsync();
                }
                catch (ObjectDisposedException)
                {
                    // The listener has been stopped.
                    return;
                }

                if (client == null) return;

                Accept(client);
            }
        }

        async void Accept(TcpClient client)
        {
            byte[] buffer = new byte[512];
            int bytesRead;
            var stream = client.GetStream();

            var headerRead = 0;
            while (headerRead < 4 && (bytesRead = await stream.ReadAsync(buffer, headerRead, 4 - headerRead)) > 0 )
            {
                headerRead += bytesRead;
            }

            if (headerRead < 4) return; // We failed to read the header.

            var bytesRemaining = BitConverter.ToInt32(buffer, 0);
            

            while (bytesRemaining > 0 && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead);

                bytesRemaining -= bytesRead;
            }

            if (bytesRemaining == 0) client.Close(); // Do a graceful shutdown
        }

        public void Stop()
        {
            cts.Cancel();
            listener.Stop();
        }
    }
}
