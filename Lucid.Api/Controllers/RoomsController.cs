using System.Threading.Tasks;
using Lucid.Api.Filters;
using Lucid.Database;
using Lucid.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lucid.Api.Controllers
{
	[Route("api/[controller]")]
	public class RoomsController : Controller
	{
		private readonly IRoomRepository _roomRepository;

		public RoomsController(IRoomRepository roomRepository)
		{
			_roomRepository = roomRepository;
		}
		
		[HttpPost]
		[ValidateModelState]
		public async Task<Room> Create([FromBody]RoomCreationRequest request)
		{
			return await _roomRepository.Create(request);
		}

		[HttpPatch]
		[ValidateModelState]
		public async Task<Room> Update([FromBody]RoomUpdateRequest request)
		{
			return await _roomRepository.Update(request);
		}
	}
}
