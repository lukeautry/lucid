using Lucid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace Lucid.Database
{
    public class Context : DbContext
    {
		public DbSet<User> Users { get; set; }
		public DbSet<Area> Areas { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<ItemDefinition> ItemDefinitions { get; set; }
		public DbSet<Item> Items { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;User Id = postgres;Password=admin;Database=lucid");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				entity.Relational().TableName = ConvertPascalCaseToSnakeCase(entity.Relational().TableName);

				foreach (var property in entity.GetProperties())
				{
					property.Relational().ColumnName = ConvertPascalCaseToSnakeCase(property.Relational().ColumnName);
				}
			}

			modelBuilder.Entity<Room>().HasOne(p => p.NorthRoom).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Room>().HasOne(p => p.EastRoom).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Room>().HasOne(p => p.SouthRoom).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Room>().HasOne(p => p.WestRoom).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Room>().HasOne(p => p.UpRoom).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Room>().HasOne(p => p.DownRoom).WithMany().OnDelete(DeleteBehavior.SetNull);
		}

	    private static string ConvertPascalCaseToSnakeCase(string value)
	    {
			return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
		}
	}
}
