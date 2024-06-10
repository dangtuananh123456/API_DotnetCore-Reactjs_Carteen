using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Domain.Entities;

namespace Infrastructure.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<OrderItem>().HasOne<Item>(x => x.Item)
				.WithMany(x => x.OrderItems)
				.HasForeignKey(x => x.ItemId);
			builder.Entity<OrderItem>().HasOne<Order>(x => x.Order)
				.WithMany(x => x.OrderItems)
				.HasForeignKey(x => x.OrderId);
		}

		public DbSet<Item> Items { set; get; }
		public DbSet<CartItem> CartItems { set; get; }
		public DbSet<Category> Categories { set; get; }
		public DbSet<Order> Orders { set; get; }
		public DbSet<OrderItem> OrderItems { set; get; }
		public DbSet<Review> Reviews { set; get; }
		public DbSet<User> Users { set; get; }
		public DbSet<Payment> Payments { set; get; }
		public DbSet<SeatReservation> SeatReservations { set; get; }
	}
}
