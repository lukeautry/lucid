﻿using System;
using System.Data;
using System.Threading.Tasks;
using Lucid.Core;
using Lucid.Database;
using Lucid.Views;

namespace Lucid.Commands
{
	public class Look : Command
	{
		private readonly IUserRepository _userRepository;
		private readonly IRoomRepository _roomRepository;

		public Look(IRedisProvider redisProvider, IUserRepository userRepository, IRoomRepository roomRepository) : base(new[] { "l", "lo", "loo", "look" }, redisProvider)
		{
			_userRepository = userRepository;
			_roomRepository = roomRepository;
		}

		public override async Task Process(string sessionId)
		{
			var session = await new SessionService(RedisProvider).Get(sessionId);
			if (!session.UserId.HasValue)
			{
				// TODO: Handle
				Console.WriteLine($"Session {sessionId} tried to look with no user ID");
				return;
			}

			var user = await _userRepository.Get(session.UserId.Value);
			if (!user.CurrentRoomId.HasValue)
			{
				Console.WriteLine($"User {user.Id} doesn't have a current room...that is a problem.");
				return;
			}

			var room = await _roomRepository.Get(user.CurrentRoomId.Value);
			await new Room(RedisProvider, room).Render(sessionId);
		}
	}
}
