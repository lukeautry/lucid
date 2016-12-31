using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Models;
using Dapper;

namespace Lucid.Database
{
	public interface IItemRepository : IRepository<Item>
	{
		Task<Item> Create(ItemCreationRequest request);
	}

	public class ItemRepository : Repository<Item>, IItemRepository
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;
		public override string TableName { get; } = "items";

		public ItemRepository(IRedisProvider redisProvider, IDbConnection connection, IUserRepository userRepository, IRoomRepository roomRepository) : base(redisProvider, connection)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
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
