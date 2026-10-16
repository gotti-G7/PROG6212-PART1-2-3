using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi1.Data;

namespace RaceDay1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseTestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DatabaseTestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            bool connected = await _context.Database.CanConnectAsync();

            if (connected)
            {
                return Ok("Connected to the RaceDay database successfully!");
            }

            return StatusCode(500, "Could not connect to the RaceDay database.");
        }
    }
}
