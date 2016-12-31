using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Broadcasts;
using Lucid.Commands;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Lucid
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var services = new ServiceCollection();
			services.AddTransient<IRedisProvider, RedisProvider>();
			services.AddTransient(typeof(IDbConnection), p =>
			{
				var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id = postgres;Password=admin;Database=lucid");
				connection.Open();

				return connection;
			});

			// TODO: This could probably be done dynamically
			services.AddTransient<IUserRepository, UserRepository>();
			services.AddTransient<IRoomRepository, RoomRepository>();
			services.AddTransient<IAreaRepository, AreaRepository>();
			services.AddTransient<IItemDefinitionRepository, ItemDefinitionRepository>();
			services.AddTransient<IItemRepository, ItemRepository>();
			services.AddTransient<ISessionUserService, SessionUserService>();
			services.AddTransient<IUserMessageQueue, UserMessageQueue>();
			services.AddTransient<ISessionService, SessionService>();
			services.AddTransient<IRoomBroadcaster, RoomBroadcaster>();

			var serviceProvider = services.BuildServiceProvider();

			CommandMap.Initialize(serviceProvider);
			StartEventQueue(serviceProvider);
			StartListener(serviceProvider);
		}

		private static void StartListener(IServiceProvider serviceProvider)
		{
			var listener = new Listener(5001, serviceProvider);
			Task.Run(async () =>
			{
				var redisProvider = serviceProvider.GetRequiredService<IRedisProvider>();
				await redisProvider.Reset();
				await listener.Listen();
			}).Wait();
		}

		private static void StartEventQueue(IServiceProvider serviceProvider)
		{
			new EventQueue(serviceProvider).Start();
		}
	}
}
