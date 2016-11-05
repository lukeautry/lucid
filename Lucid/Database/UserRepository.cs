using System.Threading.Tasks;
using Lucid.Models;
using Dapper;

namespace Lucid.Database
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User> GetByName(string name);
		Task<User> Create(User user);
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

		public async Task<User> Create(User user)
		{
			var createdUser = await Connection.QuerySingleAsync<User>(
						$"insert into {TableName}(name, hashed_password, created_at, updated_at) values (@Name, @HashedPassword, @CreatedAt, @UpdatedAt) returning *",
						new { user.Name, user.HashedPassword, user.CreatedAt, user.UpdatedAt });

			await CacheSet(GetNameCacheKey(createdUser.Name), createdUser);
			await CacheSetById(createdUser);

			return createdUser;
		}

		private static string GetNameCacheKey(string name)
		{
			return $"users:name:{name}";
		}
	}
}
