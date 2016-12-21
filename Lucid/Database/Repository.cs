using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Lucid.Core;
using Lucid.Models;

namespace Lucid.Database
{
	public interface IRepository<T> where T : Model
	{
		Task<T> Get(int id);
		Task<IEnumerable<T>> GetList(ListParams listParams = null);
	}

	public class ListParams
	{
		public readonly string WhereClause;
		public readonly object Values;

		public ListParams(string whereClause, object values)
		{
			WhereClause = whereClause;
			Values = values;
		}
	}

	public class InsertParam
	{
		public readonly string DbName;
		public readonly object Value;

		public InsertParam(string name, object value)
		{
			DbName = name;
			Value = value;
		}
	}

	public abstract class Repository<T> : IDisposable, IRepository<T> where T : Model
	{
		protected readonly IDbConnection Connection;
		private readonly IRedisProvider _redisProvider;
		public abstract string TableName { get; }

		protected Repository(IRedisProvider redisProvider, IDbConnection connection)
		{
			_redisProvider = redisProvider;

			DefaultTypeMap.MatchNamesWithUnderscores = true;
			Connection = connection;
		}

		public async Task<T> Get(int id)
		{
			var cached = await CacheGetById(id);
			if (cached != null) { return cached; }

			var model = await Connection.QueryFirstOrDefaultAsync<T>($"select * from {TableName} where Id = @Id", new { id });
			if (model != null)
			{
				await CacheSetById(model);
			}

			return model;
		}

		public async Task<IEnumerable<T>> GetList(ListParams listParams = null)
		{
			var sqlCommand = $"select * from {TableName}";
			object parameters = new { };
			if (listParams != null)
			{
				sqlCommand += $" {listParams.WhereClause}";
				parameters = listParams.Values;
			}

			return await Connection.QueryAsync<T>(sqlCommand, parameters);
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