using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Models;
using Dapper;
using Lucid.Core;

namespace Lucid.Database
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User> GetByName(string name);
		Task<User> Create(User model);
		Task<User> Modify(int userId, Action<User> modifyFunc);
	}

	public class UserRepository : Repository<User>, IUserRepository
	{
		public override string TableName => "users";

		public UserRepository(IRedisProvider redisProvider, IDbConnection connection) : base(redisProvider, connection) { }

		public async Task<User> GetByName(string name)
		{
			var cacheKey = GetNameCacheKey(name);
			var cached = await CacheGet(cacheKey);
			if (cached != null) { return cached; }

			var user = await Connection.QueryFirstOrDefaultAsync<User>($"select * from {TableName} where lower(name) = lower(@Name)", new { name });
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

		public async Task<User> Update(UserUpdateRequest user)
		{
			var updatedUser = await Connection.QuerySingleAsync<User>($"update {TableName} set name = @Name, current_room_id = @CurrentRoomId, updated_at = @UpdatedAt where id = @Id returning *", new { user.Id, user.Name, UpdatedAt = DateTime.UtcNow, user.CurrentRoomId });

			await CacheSetById(updatedUser);
			return updatedUser;
		}

		public async Task<User> Modify(int userId, Action<User> modifyFunc)
		{
			var existingUser = await Get(userId);
			modifyFunc(existingUser);

			return await Update(new UserUpdateRequest
			{
				Id = existingUser.Id,
				CurrentRoomId = existingUser.CurrentRoomId ?? 0,
				Name = existingUser.Name
			});
		}

		private static string GetNameCacheKey(string name)
		{
			return $"users:name:{name.ToLower()}";
		}
	}
}
