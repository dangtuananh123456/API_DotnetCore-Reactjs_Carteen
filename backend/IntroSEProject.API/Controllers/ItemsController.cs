using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using IntroSEProject.API.RedisCache;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace Layer.Presentation.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ItemsController : Controller
    {
        private static object _lock = new object();

		private AppDbContext dbContext;
        private IMapper mapper;
        private readonly ICacheService cacheService;

        public ItemsController(AppDbContext dbContext, IMapper mapper, ICacheService _cacheService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.cacheService = _cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0, string keyword = "", int categoryId = 0)
        {
            IEnumerable<Item> list;
            string key = categoryId.ToString() + "_" + page.ToString() + "_" + per_page.ToString() + "_" + keyword;
            var dataFromRedis = cacheService.GetData<string>(key);
            if(dataFromRedis != null)
            {
				await Console.Error.WriteLineAsync("Du lieu co san trong redis cache");

				return Ok(dataFromRedis);
            }

            //
			lock (_lock )
            {
				if (categoryId > 0)
				{
					list = dbContext.Items.Where(item => item.CategoryId == categoryId).ToList();
				}
				else if (string.IsNullOrEmpty(keyword))
				{
					list = dbContext.Items.ToList();
				}
				else
				{
					list = dbContext.Items.Where(x => x.Name.Contains(keyword)).ToList();
				}
				if (per_page == 0)
				{
					per_page = list.Count();
				}
				var model = mapper.Map<IEnumerable<ItemModel>>(list);
				var pager = new Pager<ItemModel>(model, page, per_page);
                cacheService.SetData(key, JsonConvert.SerializeObject(pager), DateTimeOffset.Now.AddMinutes(5.0));
				return Ok(pager);
			}
        }


        [HttpGet("top5")]
        public async Task<IActionResult> GetTopItem()
        {
            var dataFromRedisCache = cacheService.GetData<string>("GetTopItem");
            if(dataFromRedisCache != null)
            {
                await Console.Error.WriteLineAsync("Du lieu co san trong redis cache");
                return Ok(dataFromRedisCache);
            }
            
            lock(_lock){
                var result = dbContext.OrderItems.Where(i => i.Order.Status == "Success").GroupBy(x => x.ItemId)
                    .Select(group => new
                    {
                        Item = mapper.Map<ItemModel>(dbContext.Items.Where(i => i.Id == group.Key).SingleOrDefault()),
                        SoldQuantity = group.Sum(x => x.Quantity)
                    }).OrderByDescending(x => x.SoldQuantity)
                    .Take(5).ToList();
                cacheService.SetData("GetTopItem", JsonConvert.SerializeObject(result), DateTimeOffset.Now.AddMinutes(5.0));
                return Ok(result);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await dbContext.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var model = mapper.Map<ItemModel>(item);
            return Ok(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ItemModel model)
        {
            await Console.Out.WriteLineAsync("======================================");
            await Console.Out.WriteLineAsync(model.Name);
            var category = await dbContext.Categories.FindAsync(model.CategoryId);
            if (category == null)
            {
                return BadRequest(new { error = $"Category with id {model.CategoryId} does not exist" });
            }
            var item = mapper.Map<Item>(model);
            try
            {
                await dbContext.Items.AddAsync(item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            model.Id = item.Id;
            return Ok(model);
        }

        [Authorize(Roles = "Customer, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ItemModel model)
        {
            var item = await dbContext.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            model.Id = id;
            mapper.Map(model, item);
            try
            {
                await dbContext.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.Items.Any(e => e.Id == model.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return Ok(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await dbContext.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var model = mapper.Map<ItemModel>(item);
            dbContext.Items.Remove(item);
            await dbContext.SaveChangesAsync();
            return Ok(model);
        }
    }

}
