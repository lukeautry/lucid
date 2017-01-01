using System.Collections.Generic;
using Lucid.Core;
using Lucid.Models;

namespace Lucid.Views
{
	public sealed class Inventory : View
	{
		private readonly IEnumerable<Item> _items;

		public Inventory(IRedisProvider redisProvider, IEnumerable<Item> items) : base(redisProvider)
		{
			_items = items;
		}

		public override UserMessageBuilder Compile(UserMessageBuilder builder)
		{
			builder
				.Break()
				.Add("Inventory")
				.Add(Constants.VisualSeparator);

			foreach (var item in _items)
			{
				builder.Add(item.ItemDefinition.Name);
			}

			return builder;
		}
	}
}
