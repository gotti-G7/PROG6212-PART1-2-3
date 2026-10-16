
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RaceDayApi1.Data;
using RaceDayApi1.Models;

namespace RaceDayApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();

            return Ok(events);
        }

        // POST: api/Events
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvents),
                new { id = newEvent.EventID },
                newEvent);
        }

        // PUT: api/Events/{id}     
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            Event updatedEvent)
        {
            if (id != updatedEvent.EventID)
            {
                return BadRequest("The event ID does not match.");
            }

            var existingEvent =
                await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            existingEvent.UserID = updatedEvent.UserID;
            existingEvent.Name = updatedEvent.Name;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.RouteInfo = updatedEvent.RouteInfo;

            await _context.SaveChangesAsync();

            return Ok(existingEvent);
        }

        // DELETE: api/Events/{id}
        // Only organisers can delete events.
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var existingEvent =
                await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            bool hasCategories = await _context.EventCategories
                .AnyAsync(ec => ec.EventID == id);

            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.EventID == id);

            if (hasCategories || hasEnrolments)
            {
                return Conflict(
                    "This event cannot be deleted because it has related " +
                    "categories or enrolments. Remove those relationships first.");
            }

            _context.Events.Remove(existingEvent);

            await _context.SaveChangesAsync();

            return Ok("Event deleted successfully.");
        }
    }
}
