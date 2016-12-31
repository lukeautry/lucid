using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Commands;
using Lucid.Services;

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
	    private readonly ISessionUserService _sessionUserService;
	    private readonly IUserMessageQueue _userMessageQueue;
	    private readonly ISessionService _sessionService;

	    public ExistingUserPasswordInputEvent(
            IRedisProvider redisProvider,
            IUserRepository userRepository,
			ISessionUserService sessionUserService,
			IUserMessageQueue userMessageQueue,
			ISessionService sessionService
            ) : base("existing-user-password-input", redisProvider)
        {
            _userRepository = userRepository;
	        _sessionUserService = sessionUserService;
	        _userMessageQueue = userMessageQueue;
	        _sessionService = sessionService;
        }

        protected override async Task ExecuteBlockingEvent(ExistingUserPasswordInputEventData data)
        {
            var user = await _userRepository.Get(data.UserId);
            if (!PasswordValidation.VerifyPassword(data.Password, user.HashedPassword))
            {
                await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("That password didn't match. Please try again."));
                return;
            }
			
            var existingUserSession = await _sessionService.GetSessionByUserId(user.Id);
            if (existingUserSession != null)
            {
                await _sessionService.Evict(existingUserSession.Id);
				await _userMessageQueue.Enqueue(data.SessionId, b => b.Add("Reconnecting..."));
            }

            await _sessionService.Update(data.SessionId, s =>
            {
                s.LoginData = null;
                s.UserId = user.Id;
            });

            await Look.ShowCurrentRoom(RedisProvider, _sessionUserService, data.SessionId);
        }
    }
}
