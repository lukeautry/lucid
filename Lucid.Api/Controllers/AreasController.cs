using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Api.Filters;
using Lucid.Database;
using Lucid.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lucid.Api.Controllers
{
	[Route("api/[controller]")]
    public class AreasController : Controller
    {
	    private readonly IAreaRepository _areaRepository;
	    private readonly IRoomRepository _roomRepository;

	    public AreasController(IAreaRepository areaRepository, IRoomRepository roomRepository)
	    {
		    _areaRepository = areaRepository;
		    _roomRepository = roomRepository;
	    }
		
        [HttpGet]
        public async Task<IEnumerable<Area>> Get()
        {
	        return await _areaRepository.GetList();
        }
		
        [HttpGet("{id}")]
        public async Task<Area> Get(int id)
        {
	        return await _areaRepository.Get(id);
        }

		[HttpGet("{id}/rooms")]
		public async Task<IEnumerable<Room>> GetRooms(int id)
		{
			return await _roomRepository.GetByAreaId(id);
		}

		[HttpPost]
		[ValidateModelState]
		public async Task<Area> Post([FromBody]AreaCreationRequest request)
		{
			return await _areaRepository.Create(request);
		}

		[HttpPatch]
		[ValidateModelState]
		public async Task<Area> Update([FromBody]AreaUpdateRequest request)
		{
			return await _areaRepository.Update(request);
		}
	}
}
