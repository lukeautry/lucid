using System;

namespace Lucid.Models
{
	public class Model
	{
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}

	public abstract class ModelBuilder<T> where T : Model
	{
		public readonly T Model;

		protected ModelBuilder(T model)
		{
			model.CreatedAt = DateTime.UtcNow;
			model.UpdatedAt = DateTime.UtcNow;
			Model = model;
		}
	}
}