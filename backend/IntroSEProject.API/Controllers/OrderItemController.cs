using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace IntroSEProject.API.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderItemController :Controller
    {
        private AppDbContext dbContext;
        private IMapper mapper;
        public OrderItemController (AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0)
        {
            IEnumerable<OrderItem> list;
            list = (IEnumerable<OrderItem>)await dbContext.OrderItems.ToListAsync();
            if (per_page == 0)
            {
                per_page = list.Count();
            }
            var model = mapper.Map<IEnumerable<OrderItemModel>>(list);
            var pager = new Pager<OrderItemModel>(model, page, per_page);
            return Ok(pager);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var OrderItem = await dbContext.OrderItems.FindAsync(id);
            if (OrderItem == null)
            {
                return NotFound();
            }
            var model = mapper.Map<OrderItemModel>(OrderItem);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderItemModel model)
        {
            var Order = await dbContext.Orders.FindAsync(model.OrderId);
            if (Order == null)
            {
                return BadRequest(new { error = $"Order with id {model.OrderId} does not exist" });
            }
            var OrderItem = mapper.Map<OrderItem>(model);
            try
            {
                await dbContext.OrderItems.AddAsync(OrderItem);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            model.Id = OrderItem.Id;
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(OrderItemModel model)
        {
            var Order = await dbContext.Orders.FindAsync(model.OrderId);
            if (Order == null)
            {
                return BadRequest(new { error = $"Category with id {model.OrderId} does not exist" });
            }
            var OrderItem = mapper.Map<OrderItem>(model);
            var foundOrderItem = dbContext.OrderItems.Find(model.Id);
            if (foundOrderItem == null)
            {
                return NotFound();
            }
            dbContext.Entry(foundOrderItem).CurrentValues.SetValues(OrderItem);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.OrderItems.Any(e => e.Id == model.Id))
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
            var OrderItem = dbContext.OrderItems.Find(id);
            if (OrderItem == null)
            {
                return NotFound();
            }
            dbContext.OrderItems.Remove(OrderItem);
            await dbContext.SaveChangesAsync();
            var model = mapper.Map<OrderItemModel>(OrderItem);
            return Ok(model);
        }
    }
}
