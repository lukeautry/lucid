using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Models;
using Dapper;
using Lucid.Core;

namespace Lucid.Database
{
	public interface IAreaRepository : IRepository<Area>
	{
		Task<Area> Create(AreaCreationRequest area);
		Task<Area> Update(AreaUpdateRequest area);
	}

	public class AreaRepository : Repository<Area>, IAreaRepository
	{
		public override string TableName => "areas";

		public AreaRepository(IRedisProvider redisProvider, IDbConnection dbConnection) : base(redisProvider, dbConnection) { }

		public async Task<Area> Create(AreaCreationRequest request)
		{
			var area = new AreaBuilder(request.Name, request.Description).Model;

			var createdArea = await Connection.QuerySingleAsync<Area>(
						$"insert into {TableName}(name, description, created_at, updated_at)" +
						$"values (@Name, @Description, @CreatedAt, @UpdatedAt) returning *",
						new { area.Name, area.Description, area.CreatedAt, area.UpdatedAt });

			await CacheSetById(createdArea);
			return createdArea;
		}

		public async Task<Area> Update(AreaUpdateRequest area)
		{
			var updatedArea = await Connection.QuerySingleAsync<Area>($"update {TableName} set name = @Name, description = @Description, updated_at = @UpdatedAt where id = @Id returning *", new { area.Id, area.Name, UpdatedAt = DateTime.UtcNow, area.Description });

			await CacheSetById(updatedArea);
			return updatedArea;
		}
	}
}
