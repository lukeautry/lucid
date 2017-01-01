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
	public interface IItemRepository : IRepository<Item>
	{
		Task<Item> Create(ItemCreationRequest request);
		Task<IEnumerable<Item>> GetInventoryItems(int userId);
	}

	public class ItemRepository : Repository<Item>, IItemRepository
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;
		private readonly IItemDefinitionRepository _itemDefinitionRepository;

		public override string TableName { get; } = "items";

		public ItemRepository(
			IRedisProvider redisProvider, 
			IDbConnection connection, 
			IUserRepository userRepository, 
			IRoomRepository roomRepository,
			IItemDefinitionRepository itemDefinitionRepository
			) : base(redisProvider, connection)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
			_itemDefinitionRepository = itemDefinitionRepository;
		}

		public async Task<Item> Create(ItemCreationRequest request)
		{
			await ValidateObjectType(request.ParentObjectType, request.ParentObjectId);

			var itemDefinition = new ItemBuilder(request.ParentObjectId, request.ParentObjectType, request.ItemDefinitionId).Model;

			var createdItem = await Connection.QuerySingleAsync<Item>(
				$"insert into {TableName}(parent_object_id, parent_object_type, item_definition_id, created_at, updated_at) values (@ParentObjectId, @ParentObjectType, @ItemDefinitionId, @CreatedAt, @UpdatedAt) returning *",
				new { itemDefinition.ParentObjectId, itemDefinition.ParentObjectType, itemDefinition.ItemDefinitionId, itemDefinition.CreatedAt, itemDefinition.UpdatedAt });

			await CacheSetById(createdItem);
			return createdItem;
		}

		public async Task<IEnumerable<Item>> GetInventoryItems(int userId)
		{
			// TODO: Cache this somehow
			var items = await GetList(new ListParams("where parent_object_type = @ParentObjectType and parent_object_id = @ParentObjectId", new
			{
				ParentObjectType = ObjectType.User,
				ParentObjectId = userId
			}));

			var itemsArray = items.ToArray();
			var itemDefinitions = await _itemDefinitionRepository.GetByIds(itemsArray.Select(i => i.ItemDefinitionId).Distinct());
			var itemMap = itemDefinitions.ToDictionary(item => item.Id, item => item);

			foreach (var item in itemsArray)
			{
				item.ItemDefinition = itemMap[item.ItemDefinitionId];
			}

			return itemsArray;
		}

		private async Task ValidateObjectType(ObjectType parentObjectType, int parentObjectId)
		{
			if (parentObjectType == ObjectType.User)
			{
				var user = await _userRepository.Get(parentObjectId);
				if (user == null)
				{
					throw new Exception($"User {parentObjectId} doesn't exist, therefore it can't be used as a parent object for an item.");
				}
			}

			if (parentObjectType == ObjectType.User)
			{
				var room = await _roomRepository.Get(parentObjectId);
				if (room == null)
				{
					throw new Exception($"Room {parentObjectId} doesn't exist, therefore it can't be used as a parent object for an item.");
				}
			}
		}
	}
}
