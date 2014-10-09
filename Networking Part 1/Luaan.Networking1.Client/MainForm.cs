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

        private async void btnSend_Click(object sender, EventArgs e)
        {
            var data = UTF8Encoding.UTF8.GetBytes(tbxInput.Text);
            tbxInput.Text = string.Empty;
            var context = SynchronizationContext.Current;

            using (var client = new TcpClient())
            {
                await client.ConnectAsync("localhost", 31224).ConfigureAwait(false);

                var stream = client.GetStream();
                var buffer = new byte[512];

                await Task.WhenAll(stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, 4), stream.WriteAsync(data, 0, data.Length)).ConfigureAwait(false);

                int bytesRead;
                var decoder = UTF8Encoding.UTF8.GetDecoder();
                var charBuffer = new char[512];

                SynchronizationContext.SetSynchronizationContext(context);
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    var charsRead = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0);

                    tbxLog.AppendText(new string(charBuffer, 0, charsRead));
                }

                tbxLog.AppendText(Environment.NewLine);
            }
        }
    }
}
