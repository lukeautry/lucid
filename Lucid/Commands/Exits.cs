using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Services;

namespace Lucid.Commands
{
    public class Exits : Command
    {
	    private readonly IUserRepository _userRepository;
	    private readonly IRoomRepository _roomRepository;

	    public Exits(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "exits" }, redisProvider)
	    {
		    _userRepository = userRepository;
		    _roomRepository = roomRepository;
	    }

	    public override async Task Process(string sessionId, string[] arguments)
	    {
		    var currentRoom = await new SessionUserService(_userRepository, _roomRepository, RedisProvider).GetCurrentRoom(sessionId);
		    await new Views.Exits(RedisProvider, currentRoom).Render(sessionId);
	    }

	    public override CommandMetadata GetCommandMetadata()
	    {
		    return new CommandMetadata("Exits", "Get a list of available exits", Keys, new CommandArgument[] {});
	    }
    }
}
