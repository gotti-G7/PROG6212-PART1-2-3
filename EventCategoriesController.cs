using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi1.Data;
using RaceDayApi1.Models;

namespace RaceDayApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventCategories()
        {
            var links = await _context.EventCategories.ToListAsync();

            return Ok(links);
        }

        [HttpPost]
        public async Task<IActionResult> LinkCategoryToEvent(
       EventCategory link)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == link.EventID);

            if (!eventExists)
            {
                return NotFound("Event not found.");
            }

            bool categoryExists = await _context.Categories
            .AnyAsync(c => c.CategoryID == link.CategoryID);

            if (!categoryExists)
            {
                return NotFound("Category not found.");
            }

            bool linkExists = await _context.EventCategories
           .AnyAsync(ec =>
               ec.EventID == link.EventID &&
               ec.CategoryID == link.CategoryID);

            if (linkExists)
            {
                return Conflict(
                    "This category is already linked to this event.");
            }

            _context.EventCategories.Add(link);
            await _context.SaveChangesAsync();

            return Ok("Category linked to event successfully.");
        }

        [HttpDelete("{eventId}/{categoryId}")]
        public async Task<IActionResult> RemoveCategoryFromEvent(
            int eventId, int categoryId)
        {
            var link = await _context.EventCategories.FindAsync(
                eventId, categoryId);

            if (link == null)
            {
                return NotFound("Event-category link not found.");
            }

            _context.EventCategories.Remove(link);
            await _context.SaveChangesAsync();

            return Ok("Category unlinked from event successfully.");
        }
    }

}