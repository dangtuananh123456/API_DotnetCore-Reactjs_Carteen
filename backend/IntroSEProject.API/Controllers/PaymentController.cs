using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntroSEProject.API.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    [ApiController]
    [Route("/api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;
        public PaymentController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0, int orderId = 0, int userId = 0)
        {
            IEnumerable<Payment> list = await dbContext.Payments.ToListAsync();
            if(orderId > 0)
            {
                list = list = dbContext.Payments.Where(P => P.OrderId == orderId).ToList();
            }
            if (userId > 0)
            {
                list = list = dbContext.Payments.Where(P => P.UserId == userId).ToList();
            }
            if (per_page == 0)
            {
                per_page = list.Count();
            }
            var model = mapper.Map<IEnumerable<PaymentModel>>(list);
            var pager = new Pager<PaymentModel>(model, page, per_page);
            return Ok(pager);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            Payment? target = await dbContext.Payments.FindAsync(id);
            if (target == null)
            {
                return NotFound();
            }
            PaymentModel model = mapper.Map<PaymentModel>(target);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentModel model)
        {
            User? user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"Not exist User has id = {model.Id}" });
            }
            Order? order = await dbContext.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return BadRequest(new { error = $"Not exist Order has id = {model.OrderId}" });
            }
            Payment payment = mapper.Map<Payment>(model);
            try
            {
                await dbContext.AddAsync(payment);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            model.Id = payment.Id;
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(PaymentModel model)
        {
            Payment? target = await dbContext.Payments.FindAsync(model.Id);
            if (target == null)
            {
                return BadRequest();
            }
            User? user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new { error = $"Not exist User has id = {model.UserId}" });
            }
            Order? order = await dbContext.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return BadRequest(new { error = $"Not exist Order has Id = {model.OrderId}" });
            }
            Payment payment = mapper.Map<Payment>(model);
            try
            {
                dbContext.Entry(target).CurrentValues.SetValues(payment);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok(model);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            Payment? target = await dbContext.Payments.FindAsync(id);
            if(target == null)
            {
                return BadRequest();
            }
            dbContext.Payments.Remove(target);
            await dbContext.SaveChangesAsync();
            PaymentModel model = mapper.Map<PaymentModel>(target);
            return Ok(model);
        }
    }
}
