using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luaan.Networking1.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerMain();

            server.Start();

            Console.ReadKey();

            server.Stop();
        }
    }
}
