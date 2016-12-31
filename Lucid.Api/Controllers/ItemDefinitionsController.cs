using System.Collections.Generic;
using System.Threading.Tasks;
using Lucid.Database;
using Lucid.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lucid.Api.Controllers
{
	[Route("api/[controller]")]
	public class ItemDefinitionsController : Controller
	{
		private readonly IItemDefinitionRepository _itemDefinitionRepository;

		public ItemDefinitionsController(IItemDefinitionRepository itemDefinitionRepository)
		{
			_itemDefinitionRepository = itemDefinitionRepository;
		}

		[HttpGet]
		public async Task<IEnumerable<ItemDefinition>> Get()
		{
			return await _itemDefinitionRepository.GetList();
		}

		[HttpGet("{id}")]
		public async Task<ItemDefinition> Get(int id)
		{
			return await _itemDefinitionRepository.Get(id);
		}

		[HttpPost]
		public async Task<ItemDefinition> Post([FromBody]ItemDefinitionCreationRequest request)
		{
			return await _itemDefinitionRepository.Create(request);
		}

		[HttpPatch]
		public async Task<ItemDefinition> Patch([FromBody]ItemDefinitionUpdateRequest request)
		{
			return await _itemDefinitionRepository.Update(request);
		}

		[HttpDelete("{id}")]
		public async Task Delete(int id)
		{
			await _itemDefinitionRepository.Delete(id);
		}
	}
}
