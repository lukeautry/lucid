using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Commands
{
	public abstract class Command
	{
		public readonly string[] Keys;
		protected readonly IRedisProvider RedisProvider;

		protected Command(string[] keys, IRedisProvider redisProvider)
		{
			Keys = keys;
			RedisProvider = redisProvider;
		}

		public abstract Task Process(string sessionId);
	}
}