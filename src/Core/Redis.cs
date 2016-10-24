using StackExchange.Redis;

namespace Lucid.Core
{
	public static class RedisProvider
	{
		private static ConnectionMultiplexer _redis;
		private static ConnectionMultiplexer Redis => _redis ?? (_redis = ConnectionMultiplexer.Connect("localhost"));

		public static IDatabase GetDatabase()
		{
			return Redis.GetDatabase();
		}

		public static ISubscriber GetSubscriber()
		{
			return Redis.GetSubscriber();
		}
	}
}