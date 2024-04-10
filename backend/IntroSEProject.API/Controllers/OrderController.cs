using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace IntroSEProject.API.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderController : Controller
    {
        private AppDbContext dbContext;
        private IMapper mapper;

        public OrderController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0, int userId = 0)
        {
            IEnumerable<Order> list;
            list = await dbContext.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();
            if (userId > 0)
            {
                list = list.Where(P => P.UserId == userId).ToList();
            }
            if (per_page == 0)
            {
                per_page = list.Count();
            }
            var model = mapper.Map<IEnumerable<OrderModel>>(list);
            var pager = new Pager<OrderModel>(model, page, per_page);
            return Ok(pager);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await dbContext.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var model = mapper.Map<OrderModel>(order);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderModel model)
        {
            await Console.Out.WriteLineAsync("======================================");
            await Console.Out.WriteLineAsync(model.Status);
            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"User with id {model.UserId} does not exist" });
            }
            model.OrderDate = DateTime.Now;
            var order = mapper.Map<Order>(model);
            try
            {
                await dbContext.Orders.AddAsync(order);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            model.Id = order.Id;
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(OrderModel model)
        {
            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"Category with id {model.UserId} does not exist" });
            }
            var order = mapper.Map<Order>(model);
            var foundOrder = dbContext.Orders.Find(model.Id);
            if (foundOrder == null)
            {
                return NotFound();
            }
            dbContext.Entry(foundOrder).CurrentValues.SetValues(order);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.Orders.Any(e => e.Id == model.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return Ok(model);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = dbContext.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();
            var model = mapper.Map<OrderModel>(order);
            return Ok(model);
        }
    }
}
