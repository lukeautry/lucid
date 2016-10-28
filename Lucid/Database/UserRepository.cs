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
			return await Connection.QueryFirstAsync<User>("select * from users where name = @Name", new { name });
		}
	}
}
