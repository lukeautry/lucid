using Lucid.Database;
using Lucid.Core;
using System;
using System.Threading.Tasks;
using Lucid.Models;
using System.Linq;
using System.Collections.Generic;

namespace Lucid.Services
{
    public sealed class SessionUser
    {
        public readonly SessionData Session;
        public readonly User User;

        public SessionUser(SessionData session, User user)
        {
            Session = session;
            User = user;
        }
    }

    public sealed class SessionUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRedisProvider _redisProvider;

        public SessionUserService(IUserRepository userRepository, IRoomRepository roomRepository, IRedisProvider redisProvider)
        {
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _redisProvider = redisProvider;
        }

        public async Task<User> GetCurrentUser(string sessionId)
        {
            var sessionService = new SessionService(_redisProvider);

            var session = await sessionService.Get(sessionId);
            if (!session.UserId.HasValue)
            {
                throw new Exception($"Session {sessionId} doesn't have a valid user ID value.");
            }

            var userId = session.UserId.Value;
            return await _userRepository.Get(userId);
        }

        public async Task<IEnumerable<SessionUser>> GetSessionUsers()
        {
            var sessionService = new SessionService(_redisProvider);
            var sessions = await sessionService.GetSessions();

            var users = new List<SessionUser>();
            foreach (var session in sessions.Where(s => s.Value.UserId.HasValue))
            {
                var user = await _userRepository.Get(session.Value.UserId.Value);
                users.Add(new SessionUser(session.Value, user));
            }

            return users.ToArray();
        }

        public async Task<IEnumerable<SessionUser>> GetRoomUsers(int roomId)
        {
            var users = await GetSessionUsers();
            return users.Where(u => u.User.CurrentRoomId.HasValue && u.User.CurrentRoomId.Value == roomId).ToArray();
        }

        public async Task<Room> GetCurrentRoom(string sessionId)
        {
            var user = await GetCurrentUser(sessionId);
            if (!user.CurrentRoomId.HasValue)
            {
                throw new Exception($"Session {sessionId}, User {user.Id} doesn't have a current room ID.");
            }

            return await _roomRepository.Get(user.CurrentRoomId.Value);
        }
    }
}