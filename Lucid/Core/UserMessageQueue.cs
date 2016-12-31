using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lucid.Core
{
	public sealed class UserMessageData
	{
		public string Content { get; set; }
	}

	public interface IUserMessageQueue
	{
		Task Start(string sessionId, ISocketService socketService);
		Task Enqueue(string sessionId, Func<UserMessageBuilder, UserMessageBuilder> messageBuilderFunction);
	}

	public sealed class UserMessageQueue : IUserMessageQueue
	{
		private readonly IRedisProvider _redisProvider;

		public UserMessageQueue(IRedisProvider redisProvider)
		{
			_redisProvider = redisProvider;
		}

		public async Task Enqueue(string sessionId, Func<UserMessageBuilder, UserMessageBuilder> messageBuilderFunction)
		{
			var messageBuilder = new UserMessageBuilder();
			var content = messageBuilderFunction(messageBuilder).GetContent();

			await _redisProvider.Publish(GetKey(sessionId), new UserMessageData { Content = content });
		}

		public async Task Start(string sessionId, ISocketService socketService)
		{
			await _redisProvider.Subscribe<UserMessageData>(GetKey(sessionId), data =>
			{
				socketService.Send(data.Content);
			});
		}

		public static string GetKey(string sessionId)
		{
			return $"user-messages:{sessionId}";
		}
	}

	public sealed class UserMessageBuilder
	{
		private const string BreakValue = "\n";
		private readonly List<string> _messages = new List<string>();

		public UserMessageBuilder Add(params string[] messages)
		{
			_messages.AddRange(messages.Select(m => $"{m}{BreakValue}"));
			return this;
		}

		public UserMessageBuilder Break()
		{
			_messages.Add("\n");
			return this;
		}

		public string GetContent()
		{
			return string.Join("", _messages);
		}
	}
}
