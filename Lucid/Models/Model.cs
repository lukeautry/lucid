using System;
using System.ComponentModel.DataAnnotations;

namespace Lucid.Models
{
	public class Model
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }

		[Required]
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