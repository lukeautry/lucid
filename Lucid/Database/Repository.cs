using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Lucid.Models;
using Npgsql;

namespace Lucid.Database
{
	public abstract class Repository<T> : IDisposable where T : Model
	{
		protected IDbConnection Connection;
		public abstract string TableName { get; }

		protected Repository()
		{
			Connection = new NpgsqlConnection(string.Empty);
			Connection.Open();
		}

		public async Task<T> Get(int id)
		{
			return await Connection.QueryFirstAsync<T>($"select * from {TableName} where id = @Id", new { id });
		}

		public void Dispose()
		{
			Connection.Close();
		}
	}
}