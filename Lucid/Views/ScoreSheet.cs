using System;
using Lucid.Core;

namespace Lucid.Views
{
    public class ScoreSheet : View
    {
        private readonly Models.User _user;

        public ScoreSheet(IRedisProvider redisProvider, Models.User user) : base(redisProvider)
        {
            _user = user;
        }

        protected override Func<UserMessageBuilder, UserMessageBuilder> Compile()
        {
            return builder => builder
                    .Break()
                    .Add($"Name: {_user.Name}");
        }
    }
}
