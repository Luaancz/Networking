using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luaan.Networking2.Client
{
	public static class CommonHelper
	{
		struct Unit { }

		public static Task AsTask(this CancellationToken @this)
		{
			var tcs = new TaskCompletionSource<Unit>();

			@this.Register(() => tcs.SetResult(default(Unit)));

			return tcs.Task;
		}
	}
}
