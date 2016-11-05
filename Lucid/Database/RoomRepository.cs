using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Models;
using Dapper;

namespace Lucid.Database
{
	public interface IRoomRepository : IRepository<Room>
	{
		Task<IEnumerable<Room>> GetByAreaId(int areaId);
		Task<Room> Create(RoomCreationRequest room);
		Task<Room> Update(RoomUpdateRequest room);
	}

	public class RoomRepository : Repository<Room>, IRoomRepository
	{
		public override string TableName => "rooms";

		public async Task<IEnumerable<Room>> GetByAreaId(int areaId)
		{
			return await GetList(new ListParams("where area_id = @AreaId", new { AreaId = areaId }));
		}

		public async Task<Room> Create(RoomCreationRequest request)
		{
			var room = new RoomBuilder(request.Name, request.Description, request.AreaId, request.NorthRoomId, request.EastRoomId, request.SouthRoomId, request.WestRoomId, request.UpRoomId, request.DownRoomId).Model;

			var createdRoom = await Connection.QuerySingleAsync<Room>(
						$"insert into {TableName}(name, description, created_at, updated_at, north_room_id, east_room_id, south_room_id, west_room_id, up_room_id, down_room_id, area_id) " +
						$"values (@Name, @Description, @CreatedAt, @UpdatedAt, @NorthRoomId, @EastRoomId, @SouthRoomId, @WestRoomId, @UpRoomId, @DownRoomId, @AreaId) returning *",
						new { room.Name, room.Description, room.CreatedAt, room.UpdatedAt, room.NorthRoomId, room.EastRoomId, room.SouthRoomId, room.WestRoomId, room.UpRoomId, room.DownRoomId, room.AreaId });

			await CacheSetById(createdRoom);
			return createdRoom;
		}

		public async Task<Room> Update(RoomUpdateRequest request)
		{
			var updatedRoom = await Connection.QuerySingleAsync<Room>(
				$"update {TableName} set name = @Name, description = @Description, updated_at = @UpdatedAt, north_room_id = @NorthRoomId, east_room_id = @EastRoomId, south_room_id = @SouthRoomId, west_room_id = @WestRoomId," +
				$"up_room_id = @UpRoomId, down_room_id = @DownRoomId where id = @Id returning *", 
				new { request.Name, request.Description, UpdatedAt = DateTime.UtcNow, request.NorthRoomId, request.EastRoomId, request.SouthRoomId, request.WestRoomId, request.UpRoomId, request.DownRoomId, request.Id });

			await CacheSetById(updatedRoom);
			return updatedRoom;
		}
	}
}
