using System.Threading.Tasks;
using Lucid.Commands;
using Lucid.Core;
using Lucid.Database;
using Lucid.Models;

namespace Lucid.Events
{
    public class NewUserConfirmPasswordInputEventData : BlockingEventData
    {
        public readonly string Password;

        public NewUserConfirmPasswordInputEventData(string sessionId, string password) : base(sessionId)
        {
            Password = password;
        }
    }

    public class NewUserConfirmPasswordInputEvent : BlockingEvent<NewUserConfirmPasswordInputEventData>
    {
        private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;
        public const string NonMatchingPasswordText = "Passwords must match.";

        public NewUserConfirmPasswordInputEvent(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository)
        : base("new-user-confirm-password-input-event", redisProvider)
        {
            _userRepository = userRepository;
			_roomRepository = roomRepository;
        }

        protected override async Task ExecuteBlockingEvent(NewUserConfirmPasswordInputEventData data)
        {
            var userMessageQueue = new UserMessageQueue(RedisProvider);
            var sessionService = new SessionService(RedisProvider);
            var session = await sessionService.Get(data.SessionId);

            if (data.Password != session.CreationData.Password)
            {
                await userMessageQueue.Enqueue(session.Id, b => b.Add(NonMatchingPasswordText).Break().Add(NameInputEvent.EnterPasswordText));
                await sessionService.Update(session.Id, s =>
                {
                    s.CreationData.Password = null;
                    s.CreationData.ConfirmPasswordInputPending = false;
                    s.CreationData.PasswordInputPending = true;
                });
                return;
            }

            var model = new UserBuilder(session.CreationData.Name, PasswordValidation.HashPassword(session.CreationData.Password)).Model;
            model.CurrentRoomId = 1; // TODO: Should this be a global setting of some sort?

            var user = await _userRepository.Create(model);
			await sessionService.Update(session.Id, s => {
				s.UserId = user.Id;
				s.CreationData = null;
			});
            await Look.ShowCurrentRoom(_userRepository, _roomRepository, RedisProvider, data.SessionId);
        }
    }
}
