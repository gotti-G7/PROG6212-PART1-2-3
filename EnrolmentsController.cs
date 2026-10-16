using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi1.Data;
using RaceDayApi1.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RaceDayApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrolmentsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEnrolments()
        {
            var enrolments = await _context.Enrolments.ToListAsync();

            return Ok(enrolments);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrolment(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            return Ok(enrolment);
        }

       
[Authorize(Roles = "Participant")]
[HttpPost]
public async Task<IActionResult> CreateEnrolment(
    Enrolments newEnrolment)
        {
       
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) ||
                !int.TryParse(userIdClaim, out int authenticatedUserId))
            {
                return Unauthorized();
            }

           
            if (newEnrolment.UserID != authenticatedUserId)
            {
                return Forbid();
            }

          
            bool userExists = await _context.Users
                .AnyAsync(u => u.UserID == authenticatedUserId);

            if (!userExists)
            {
                return NotFound("User not found.");
            }

           
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == newEnrolment.EventID);

            if (!eventExists)
            {
                return NotFound("Event not found.");
            }

          
            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryID == newEnrolment.CategoryID);

            if (!categoryExists)
            {
                return NotFound("Category not found.");
            }

           
            bool categoryLinked = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.EventID == newEnrolment.EventID &&
                    ec.CategoryID == newEnrolment.CategoryID);

            if (!categoryLinked)
            {
                return BadRequest(
                    "This category is not linked to the selected event.");
            }

            
            bool alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.EventID == newEnrolment.EventID &&
                    e.CategoryID == newEnrolment.CategoryID &&
                    e.UserID == authenticatedUserId);

            if (alreadyEnrolled)
            {
                return Conflict(
                    "This participant is already enrolled in this event category.");
            }

            
            if (string.IsNullOrWhiteSpace(newEnrolment.Status))
            {
                newEnrolment.Status = "Registered";
            }

            
            _context.Enrolments.Add(newEnrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = newEnrolment.EnrolmentID },
                newEnrolment);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrolment(
            int id, Enrolments updatedEnrolment)
        {
            if (id != updatedEnrolment.EnrolmentID)
            {
                return BadRequest(
                    "The enrolment ID does not match.");
            }

            var existingEnrolment =
                await _context.Enrolments.FindAsync(id);

            if (existingEnrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            bool userExists = await _context.Users
                .AnyAsync(u => u.UserID == updatedEnrolment.UserID);

            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == updatedEnrolment.EventID);

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryID == updatedEnrolment.CategoryID);

            if (!userExists || !eventExists || !categoryExists)
            {
                return BadRequest(
                    "The user, event, or category does not exist.");
            }

            bool categoryLinked = await _context.EventCategories
                .AnyAsync(ec =>
                    ec.EventID == updatedEnrolment.EventID &&
                    ec.CategoryID == updatedEnrolment.CategoryID);

            if (!categoryLinked)
            {
                return BadRequest(
                    "This category is not linked to the selected event.");
            }

            existingEnrolment.UserID = updatedEnrolment.UserID;
            existingEnrolment.EventID = updatedEnrolment.EventID;
            existingEnrolment.CategoryID = updatedEnrolment.CategoryID;
            existingEnrolment.Status = updatedEnrolment.Status;

            await _context.SaveChangesAsync();

            return Ok(existingEnrolment);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            bool hasResults = await _context.Results
                .AnyAsync(r => r.EnrolmentID == id);

            if (hasResults)
            {
                return Conflict(
                    "This enrolment cannot be deleted because it has a recorded result.");
            }

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok("Enrolment deleted successfully.");
        }
    }
}
