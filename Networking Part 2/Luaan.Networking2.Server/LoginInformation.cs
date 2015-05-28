using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luaan.Networking2.Server
{
	public sealed class LoginInformation
	{
		private readonly string name;

		public LoginInformation (string name)
		{
			this.name = name;
		}

		public string Name { get { return this.name; } }
	}
}
