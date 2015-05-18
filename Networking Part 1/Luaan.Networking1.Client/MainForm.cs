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

namespace Luaan.Networking1.Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

		private async Task SendAndWaitForResponse()
		{
			var data = UTF8Encoding.UTF8.GetBytes(tbxInput.Text);
			tbxInput.Text = string.Empty;

			using (var client = new TcpClient())
			{
				await client.ConnectAsync("localhost", 31224).ConfigureAwait(false);

				var stream = client.GetStream();
				var buffer = new byte[512];

				// Why the ConfigureAwait(false) here? It's possible (though *highly* unlikely) that the preceding await completed synchronously.
				// That would mean we'd actually be continuing on the GUI thread, which we don't want - yet.
				await Task.WhenAll(stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, 4), stream.WriteAsync(data, 0, data.Length)).ConfigureAwait(false);

				int bytesRead;
				// Decoder is useful in properly handling multi-byte character encodings - it will only emit "complete" characters, so we're not going to
				// mangle multi-byte characters by accident. Do not use something like StreamReader - it would work for our exact scenario, but it breaks down
				// on more complicated streams.
				var decoder = UTF8Encoding.UTF8.GetDecoder();
				var charBuffer = new char[512];

				// Get back on the GUI thread - we need to modify the tbxLog control, which can only be done from the GUI thread.
				while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(true)) != 0)
				{
					var charsRead = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0);

					tbxLog.AppendText(new string(charBuffer, 0, charsRead));
				}

				tbxLog.AppendText(Environment.NewLine);
			}
		}

        private async void btnSend_Click(object sender, EventArgs e)
        {
			try
			{
				// Let's make sure the UI prohibits sending another message concurrently.
				// This is not strictly necessary (we can handly any number of concurrent sends), but it will avoid mixing the different responses in the log.
				btnSend.Enabled = false;

				await SendAndWaitForResponse();
			}
			// This would usually have a catch-all for a typical GUI client application, putting the exception in log. I'm omitting this to preserve exception details in the debugger.
			finally
			{
				btnSend.Enabled = true;
			}
        }
    }
}
