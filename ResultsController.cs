
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi1.Data;
using RaceDayApi1.Models;


namespace RaceDayApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResultsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.Results.ToListAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
                return NotFound("Result not found.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResult(Result newResult)
        {
            bool enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentID == newResult.EnrolmentID);

            if (!enrolmentExists)
                return BadRequest("Enrolment not found.");

            _context.Results.Add(newResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = newResult.ResultID },
                newResult);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(
           int id, Result updatedResult)
        {
            if (id != updatedResult.ResultID)
                return BadRequest("The result ID does not match.");

            var existingResult = await _context.Results.FindAsync(id);

            if (existingResult == null)
                return NotFound("Result not found.");

            bool enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentID == updatedResult.EnrolmentID);

            if (!enrolmentExists)
                return BadRequest("Enrolment not found.");

            existingResult.EnrolmentID = updatedResult.EnrolmentID;
            existingResult.FinishTime = updatedResult.FinishTime;
            existingResult.Position = updatedResult.Position;
            existingResult.RecordedAt = updatedResult.RecordedAt;

            await _context.SaveChangesAsync();

            return Ok(existingResult);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
                return NotFound("Result not found.");

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok("Result deleted successfully.");
        }
    
    }
}
