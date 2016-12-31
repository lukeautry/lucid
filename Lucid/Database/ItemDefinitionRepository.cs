using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Models;
using Dapper;

namespace Lucid.Database
{
	public interface IItemDefinitionRepository : IRepository<ItemDefinition>
	{
		Task<ItemDefinition> Create(ItemDefinitionCreationRequest itemDefinition);
		Task<ItemDefinition> Update(ItemDefinitionUpdateRequest itemDefinition);
	}

	public class ItemDefinitionRepository : Repository<ItemDefinition>, IItemDefinitionRepository
	{
		public override string TableName { get; } = "item_definitions";

		public ItemDefinitionRepository(IRedisProvider redisProvider, IDbConnection connection) : base(redisProvider, connection) { }

		public async Task<ItemDefinition> Create(ItemDefinitionCreationRequest request)
		{
			var itemDefinition = new ItemDefinitionBuilder(request.Name, request.Description).Model;

			var createdItemDefinition = await Connection.QuerySingleAsync<ItemDefinition>(
				$"insert into {TableName}(name, description, created_at, updated_at) values (@Name, @Description, @CreatedAt, @UpdatedAt) returning *",
				new { itemDefinition.Name, itemDefinition.Description, itemDefinition.CreatedAt, itemDefinition.UpdatedAt });

			await CacheSetById(createdItemDefinition);
			return createdItemDefinition;
		}

		public async Task<ItemDefinition> Update(ItemDefinitionUpdateRequest request)
		{
			var updatedItemDefinition = await Connection.QuerySingleAsync<ItemDefinition>(
				$"update {TableName} set name = @Name, description = @Description, updated_at = @UpdatedAt where id = @Id returning *",
				new { request.Name, request.Description, UpdatedAt = DateTime.UtcNow, request.Id });

			await CacheSetById(updatedItemDefinition);
			return updatedItemDefinition;
		}
	}
}
