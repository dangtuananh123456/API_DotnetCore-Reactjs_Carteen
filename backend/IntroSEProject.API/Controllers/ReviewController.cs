using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntroSEProject.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ReviewController : Controller
    {
        private AppDbContext dbContext;
        private IMapper mapper;
        public ReviewController (AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]

        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0)
        {
            IEnumerable<Review> list;
            list = (IEnumerable<Review>)await dbContext.Reviews.ToListAsync();
            if (per_page == 0)
            {
                per_page = list.Count();
            }
            var model = mapper.Map<IEnumerable<ReviewModel>>(list);
            var pager = new Pager<ReviewModel>(model, page, per_page);
            return Ok(pager);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await dbContext.Reviews.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var model = mapper.Map<ReviewModel>(order);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewModel model)
        {
            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"User with id {model.UserId} does not exist" });
            }
            var orderItem = await dbContext.OrderItems.FindAsync(model.OrderItemId);
            if(orderItem == null)
            {
                return BadRequest(new { error = $"OrderItem has id = {model.OrderItemId} not exist" });
            }
            var order = mapper.Map<Review>(model);
            try
            {
                await dbContext.Reviews.AddAsync(order);
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
        public async Task<IActionResult> Edit(ReviewModel model)
        {
            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"User with id {model.UserId} does not exist" });
            }

            var orderItem = await dbContext.OrderItems.FindAsync(model.OrderItemId);
            if(orderItem == null)
            {
                return BadRequest(new { error = $"OrderItem with id = {model.OrderItemId} not exist" });
            }
            var order = mapper.Map<Review>(model);
            var foundReview = dbContext.Reviews.Find(model.Id);
            if (foundReview == null)
            {
                return NotFound();
            }
            dbContext.Entry(foundReview).CurrentValues.SetValues(order);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.Reviews.Any(e => e.Id == model.Id))
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
            var order = dbContext.Reviews.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            dbContext.Reviews.Remove(order);
            await dbContext.SaveChangesAsync();
            var model = mapper.Map<ReviewModel>(order);
            return Ok(model);
        }
    }
}
