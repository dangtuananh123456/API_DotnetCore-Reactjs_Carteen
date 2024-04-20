using Infrastructure.Data;
using Layer.Domain.Entities;
using Layer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
	public class ItemRepository : IItemRepository
	{
		private readonly AppDbContext dbContext;

		public ItemRepository(AppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<object> GetTopItem()
		{
			var result = dbContext.OrderItems.Where(i => i.Order.Status == "Success").GroupBy(x => x.ItemId)
			.Select(group => new
			{
						Item = mapper.Map<ItemModel>(dbContext.Items.Where(i => i.Id == group.Key).SingleOrDefault()),
						SoldQuantity = group.Sum(x => x.Quantity)
					}).OrderByDescending(x => x.SoldQuantity)
					.Take(5).ToList();
		}
	}
}
