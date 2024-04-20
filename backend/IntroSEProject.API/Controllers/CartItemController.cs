using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.RedisCache;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Layer.Presentation.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    [ApiController]
    [Route("/api/[controller]")]
    public class CartItemController : Controller
    {
        private AppDbContext dbContext;
        private IMapper mapper;
        private readonly ICacheService cacheService;
        public CartItemController(AppDbContext dbContext, IMapper mapper, ICacheService _cacheService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.cacheService = _cacheService;
        }
        [HttpGet]
        public IActionResult GetPaging(int userId = -1)
        {
            if (userId <= 0)
            {
                return BadRequest();
            }
            //IEnumerable<CartItem> cartItems = dbContext.CartItems.Where(c => c.UserId == userId);
            var list = (from cartItem in dbContext.CartItems 
                        join item in dbContext.Items on cartItem.ItemId equals item.Id
                        where cartItem.UserId == userId
                        select new { id = mapper.Map<CartItemModel>(cartItem).Id, quantity = mapper.Map<CartItemModel>(cartItem).Quantity,
                           itemId = mapper.Map<CartItemModel>(cartItem).ItemId, userId = mapper.Map<CartItemModel>(cartItem).UserId, item = mapper.Map<ItemModel>(item)}
                        ).ToList();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await dbContext.CartItems.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var model = mapper.Map<CartItemModel>(order);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CartItemModel model)
        {
            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"User with id {model.UserId} does not exist" });
            }
            var item = await dbContext.Items.FindAsync(model.ItemId);
            if(item == null)
            {
                return BadRequest(new {error = $"Item has id = {model.ItemId} not exist" });
            }
            var cartItem = await dbContext.CartItems.Where(c => c.ItemId == model.ItemId).FirstOrDefaultAsync();
            if(cartItem != null)
            {
                var tmp = cartItem;
                tmp.Quantity = tmp.Quantity + model.Quantity;
                dbContext.Entry(cartItem).CurrentValues.SetValues(tmp);
                await dbContext.SaveChangesAsync();
                return Ok(mapper.Map<CartItemModel>(tmp));
            }
            var order = mapper.Map<CartItem>(model);
            try
            {
                await dbContext.CartItems.AddAsync(order);
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
        public async Task<IActionResult> Edit(CartItemModel model)
        {
            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"Category with id {model.UserId} does not exist" });
            }
            var item = await dbContext.Items.FindAsync(model.ItemId);
            if (item == null)
            {
                return BadRequest(new { error = $"Item has id = {model.ItemId} not exist" });
            }
            var order = mapper.Map<CartItem>(model);
            var foundCartItem = dbContext.CartItems.Find(model.Id);
            if (foundCartItem == null)
            {
                return NotFound();
            }
            dbContext.Entry(foundCartItem).CurrentValues.SetValues(order);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.CartItems.Any(e => e.Id == model.Id))
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
            var order = dbContext.CartItems.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            dbContext.CartItems.Remove(order);
            await dbContext.SaveChangesAsync();
            var model = mapper.Map<CartItemModel>(order);
            return Ok(model);
        }
    }
}
