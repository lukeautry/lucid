using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Lucid.Core;
using Lucid.Models;
using Npgsql;

namespace Lucid.Database
{
	public abstract class Repository<T> : IDisposable where T : Model
	{
		protected readonly IDbConnection Connection;
		private readonly IRedisProvider _redisProvider;
		public abstract string TableName { get; }

		protected Repository(IRedisProvider redisProvider = null)
		{
			_redisProvider = redisProvider ?? new RedisProvider();

			DefaultTypeMap.MatchNamesWithUnderscores = true;
			Connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id = postgres;Password=admin;Database=lucid");
			Connection.Open();
		}

		public async Task<T> Get(int id)
		{
			var cached = await CacheGetById(id);
			if (cached != null) {
				return cached;
			}

			var model = await Connection.QueryFirstOrDefaultAsync<T>($"select * from {TableName} where id = @Id", new { id });
			await CacheSetById(model);

			return model;
		}

		public void Dispose()
		{
			Connection.Close();
		}

		protected async Task<T> CacheGetById(int id)
		{
			return await CacheGet(GetCacheKey(id));
		}

		protected async Task CacheSetById(T model)
		{
			await CacheSet(GetCacheKey(model.Id), model);
		}

		protected async Task<T> CacheGet(string key)
		{
			return await _redisProvider.GetObject<T>(key);
		}

		protected async Task CacheSet(string key, T model)
		{
			await _redisProvider.SetObject(key, model);
		}

		private string GetCacheKey(int id)
		{
			return $"{TableName}:{id}";
		}
	}
}