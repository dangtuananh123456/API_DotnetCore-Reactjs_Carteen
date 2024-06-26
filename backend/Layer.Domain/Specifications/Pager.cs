﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Domain.Specifications
{
	public class Pager<T>
	{
		public int page { get; set; }
		public int per_page { get; set; }
		public int total { get; set; }
		public int total_pages { get; set; }
		public IEnumerable<T>? data { get; set; }
		public Pager(IEnumerable<T> allItems, int page, int per_page)
		{
			this.page = page;
			total = allItems.Count();
			if (total == 0)
			{
				return;
			}
			this.per_page = per_page;
			total_pages = (int)Math.Ceiling((decimal)total / per_page);
			data = allItems.Skip((page - 1) * per_page).Take(per_page);
		}
	}
}
