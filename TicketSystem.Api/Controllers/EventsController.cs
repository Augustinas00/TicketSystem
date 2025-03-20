using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Api.Data;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly TicketDbContext _context;

        public EventsController(TicketDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(string? search, int? categoryId, string? city)
        {
            var query = _context.Events.Include(e => e.Category).Include(e => e.Tickets).ThenInclude(t => t.Category).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search));
            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);
            if (!string.IsNullOrEmpty(city))
                query = query.Where(e => e.City == city);

            var events = await query.ToListAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var evt = await _context.Events.Include(e => e.Category).Include(e => e.Tickets).ThenInclude(t => t.Category).FirstOrDefaultAsync(e => e.Id == id);
            if (evt == null) return NotFound();
            return Ok(evt);
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyTicket(int id, [FromBody] BuyTicketRequest request)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.EventId == id && t.Row == request.Row && t.Seat == request.Seat);
            if (ticket == null) return NotFound();
            if (ticket.IsSold) return BadRequest("Ticket already sold.");

            ticket.IsSold = true;
            await _context.SaveChangesAsync();
            return Ok(ticket);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateDto evt)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(evt.Name) || evt.CategoryId <= 0)
            {
                return BadRequest("Invalid event data. Name and valid CategoryId are required.");
            }

            var newEvent = new Event
            {
                Name = evt.Name,
                EventDate = evt.EventDate,
                Venue = evt.Venue,
                City = evt.City,
                Description = evt.Description,
                ImageUrl = evt.ImageUrl,
                CategoryId = evt.CategoryId,
                Tickets = new List<Ticket>()
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();
            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class BuyTicketRequest
    {
        public int Row { get; set; }
        public int Seat { get; set; }
    }

    public class EventCreateDto
    {
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}