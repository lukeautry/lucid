using System.Threading.Tasks;
using Lucid.Core;

namespace Lucid.Commands
{
    public class Help : Command
    {
        public Help(IRedisProvider redisProvider) : base(new[] { "h", "he", "hel", "help" }, redisProvider) { }

        public override CommandMetadata GetCommandMetadata()
        {
            return new CommandMetadata("Help", "Find help articles", Keys, new[]{
                new CommandArgument("topic/command", true)
            });
        }

        public override async Task Process(string sessionId, string[] arguments)
        {
            if (arguments.Length == 0)
            {
                await new Views.Help(RedisProvider).Render(sessionId);
                return;
            }

            var argument = arguments[0];

            var command = CommandMap.Find(argument);
            if (command != null)
            {
                await command.DisplayHelpText(sessionId);
            }
        }


    }
}
