using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntroSEProject.API.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    [ApiController]
    [Route("/api/[controller]")]
    public class SeatReservationController : Controller
    {
        private AppDbContext dbContext;
        private IMapper mapper;
        public SeatReservationController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0)
        {
            IEnumerable<SeatReservation> list;
            list = (IEnumerable<SeatReservation>)await dbContext.SeatReservations.ToListAsync();
            if (per_page == 0)
            {
                per_page = list.Count();
            }
            var model = mapper.Map<IEnumerable<SeatReservationModel>>(list);
            var pager = new Pager<SeatReservationModel>(model, page, per_page);
            return Ok(pager);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seatReservation = await dbContext.SeatReservations.FindAsync(id);
            if (seatReservation == null)
            {
                return NotFound();
            }
            var model = mapper.Map<SeatReservationModel>(seatReservation);  
            return Ok(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(SeatReservationModel model)
        {
            var order = await dbContext.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return BadRequest(new { error = $"Order with id {model.OrderId} does not exist" });
            }
            var seatReservation = mapper.Map<SeatReservation>(model);
            try
            {
                await dbContext.SeatReservations.AddAsync(seatReservation);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            model.Id = seatReservation.Id;
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(SeatReservationModel model)
        {
            var order = await dbContext.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return BadRequest(new { error = $"Category with id {model.OrderId} does not exist" });
            }
            var seatReservation = mapper.Map<SeatReservation>(model);
            var foundSeatReservation = dbContext.SeatReservations.Find(model.Id);
            if (foundSeatReservation == null)
            {
                return NotFound();
            }
            dbContext.Entry(foundSeatReservation).CurrentValues.SetValues(seatReservation);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.SeatReservations.Any(e => e.Id == model.Id))
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
            var seatReservation = dbContext.SeatReservations.Find(id);
            if (seatReservation == null)
            {
                return NotFound();
            }
            dbContext.SeatReservations.Remove(seatReservation);
            await dbContext.SaveChangesAsync();
            var model = mapper.Map<SeatReservationModel>(seatReservation);
            return Ok(model);
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteBySeat(int seatNumber = 0)
        {
            var seatReservation = dbContext.SeatReservations.FirstOrDefault(seat => seat.SeatNumber == seatNumber);
            if (seatReservation == null)
            {
                return NotFound();
            }
            dbContext.SeatReservations.Remove(seatReservation);
            await dbContext.SaveChangesAsync();
            var model = mapper.Map<SeatReservationModel>(seatReservation);
            return Ok(model);
        }
    }
}
