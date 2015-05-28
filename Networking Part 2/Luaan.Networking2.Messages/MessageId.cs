using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luaan.Networking2.Messages
{
	public enum MessageId : byte
	{
		Unknown = 0x00,

		LoginRequest = 0x01,
		ChatMessage = 0x02,
	}
}
