using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Task.Run(async () =>
			{
				var listener = new Listener(5000);
				await listener.Listen();
			}).Wait();
		}
	}
}
