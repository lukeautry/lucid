using System.Threading.Tasks;
using Lucid.Database;
using Lucid.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lucid.Api.Controllers
{
	[Route("api/[controller]")]
	public class ItemsController : Controller
	{
		private readonly IItemRepository _itemDefinitionRepository;

		public ItemsController(IItemRepository itemDefinitionRepository)
		{
			_itemDefinitionRepository = itemDefinitionRepository;
		}

		[HttpPost]
		public async Task<Item> Post([FromBody] ItemCreationRequest request)
		{
			return await _itemDefinitionRepository.Create(request);
		}
	}
}