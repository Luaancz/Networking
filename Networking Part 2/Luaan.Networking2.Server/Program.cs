using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luaan.Networking2.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new ServerMain();

			server.Start();
			Console.WriteLine("Listening...");
			Console.WriteLine("Press any key to exit");

			Console.ReadKey();

			server.Stop();
		}
	}
}
