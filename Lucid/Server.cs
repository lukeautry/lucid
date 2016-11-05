using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid
{
	public class Program
	{
		public static void Main(string[] args)
		{
			new EventQueue().Start();

			Task.Run(async () =>
			{
				var listener = new Listener(5001);
				await listener.Listen();
			}).Wait();
		}
	}
}
