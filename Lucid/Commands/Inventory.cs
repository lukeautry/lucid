using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;

namespace Lucid.Commands
{
	public sealed class Inventory : Command
	{
		private readonly IItemRepository _itemRepository;
		private readonly ISessionUserService _sessionUserService;

		public Inventory(IRedisProvider redisProvider, IItemRepository itemRepository, ISessionUserService sessionUserService) 
			: base(new[] { "i", "in", "inv", "inve", "inven", "invent", "invento", "inventor", "inventory" }, redisProvider)
		{
			_itemRepository = itemRepository;
			_sessionUserService = sessionUserService;
		}

		public override async Task Process(string sessionId, string[] arguments)
		{
			var user = _sessionUserService.GetCurrentUser(sessionId);
		}

		public override CommandMetadata GetCommandMetadata()
		{
			return new CommandMetadata("Inventory", "View a list of your items", Keys, new CommandArgument[] { });
		}
	}
}
