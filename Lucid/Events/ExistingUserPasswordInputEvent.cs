using System;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;
using Lucid.Commands;

namespace Lucid.Events
{
    public class ExistingUserPasswordInputEventData : BlockingEventData
    {
        public readonly string Password;
        public readonly int UserId;

        public ExistingUserPasswordInputEventData(string sessionId, int userId, string password) : base(sessionId)
        {
            UserId = userId;
            Password = password;
        }
    }

    public class ExistingUserPasswordInputEvent : BlockingEvent<ExistingUserPasswordInputEventData>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserMessageQueue _userMessageQueue;
        private readonly IRoomRepository _roomRepository;

        public ExistingUserPasswordInputEvent(
            IRedisProvider redisProvider,
            IUserRepository userRepository,
            IRoomRepository roomRepository
            ) : base("existing-user-password-input", redisProvider)
        {
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _userMessageQueue = new UserMessageQueue(RedisProvider);
        }

        protected override async Task ExecuteBlockingEvent(ExistingUserPasswordInputEventData data)
        {
            var user = await _userRepository.Get(data.UserId);
            if (!PasswordValidation.VerifyPassword(data.Password, user.HashedPassword))
            {
                await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("That password didn't match. Please try again."));
                return;
            }

            var sessionService = new SessionService(RedisProvider);

            var existingUserSession = await sessionService.GetSessionByUserId(user.Id);
            if (existingUserSession != null)
            {
                await sessionService.Evict(existingUserSession.Id);
				await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("Reconnecting..."));
            }

            await new SessionService(RedisProvider).Update(data.SessionId, s =>
            {
                s.LoginData = null;
                s.UserId = user.Id;
            });

            await Look.ShowCurrentRoom(_userRepository, _roomRepository, RedisProvider, data.SessionId);
        }
    }
}
