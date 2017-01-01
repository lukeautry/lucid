using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Models;
using Dapper;
using System.Linq;

namespace Lucid.Database
{
	public interface IItemDefinitionRepository : IRepository<ItemDefinition>
	{
		Task<ItemDefinition> Create(ItemDefinitionCreationRequest itemDefinition);
		Task<ItemDefinition> Update(ItemDefinitionUpdateRequest itemDefinition);
		Task<IEnumerable<ItemDefinition>> GetByIds(IEnumerable<int> distinct);
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

		public async Task<IEnumerable<ItemDefinition>> GetByIds(IEnumerable<int> ids)
		{
			var cachedItemDefinitions = new List<ItemDefinition>();

			var queryIds = new List<int>();
			foreach (var id in ids)
			{
				var itemDefinition = await CacheGetById(id);
				if (itemDefinition != null)
				{
					cachedItemDefinitions.Add(itemDefinition);
					continue;
				}

				queryIds.Add(id);
			}


			if (!queryIds.Any()) { return cachedItemDefinitions; }

			var fetchedItemDefinitions = await GetList(new ListParams("where id in @Ids", new
			{
				Ids = queryIds.ToArray()
			}));

			foreach (var fetchedItemDefinition in fetchedItemDefinitions)
			{
				await CacheSetById(fetchedItemDefinition);
				cachedItemDefinitions.Add(fetchedItemDefinition);
			}

			return cachedItemDefinitions;
		}
	}
}
