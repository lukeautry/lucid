using System.Threading.Tasks;
using Lucid.Models;
using Dapper;

namespace Lucid.Database
{
	public interface IUserRepository
	{
		Task<User> Get(int id);
		Task<User> GetByName(string name);
	}

	public class UserRepository : Repository<User>, IUserRepository
	{
		public override string TableName => "users";

		public async Task<User> GetByName(string name)
		{
			var cacheKey = GetNameCacheKey(name);
			var cached = await CacheGet(cacheKey);
			if (cached != null) { return cached; }

			var user = await Connection.QueryFirstOrDefaultAsync<User>($"select * from {TableName} where name = @Name", new { name });
			if (user != null) { await CacheSet(cacheKey, user); }

			return user;
		}

		private static string GetNameCacheKey(string name)
		{
			return $"users:name:${name}";
		}
	}
}
