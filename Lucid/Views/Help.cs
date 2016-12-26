using System;
using Lucid.Core;

namespace Lucid.Views
{
    public class Help : View
    {
        public Help(IRedisProvider redisProvider) : base(redisProvider) { }

        protected override Func<UserMessageBuilder, UserMessageBuilder> Compile()
        {
            return builder => builder
                    .Break()
                    .Add($"Usage: help [topic or command], e.g. help look");
        }
    }
}
