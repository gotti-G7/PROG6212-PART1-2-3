using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayApi1.Data;
using RaceDayApi1.Models;

namespace RaceDayApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(
        Category newCategory)
        {
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = newCategory.CategoryID },
                newCategory);
        }
        /// 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
       int id, Category updatedCategory)
        {
            if (id != updatedCategory.CategoryID)
            {
                return BadRequest(
                    "The category ID does not match.");
            }

            var existingCategory =
                await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            existingCategory.Name = updatedCategory.Name;
            existingCategory.DistanceKM =
                updatedCategory.DistanceKM;
            existingCategory.Description =
                updatedCategory.Description;

            await _context.SaveChangesAsync();

            return Ok(existingCategory);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category =
                await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            bool hasEventLinks = await _context.EventCategories
                .AnyAsync(ec => ec.CategoryID == id);

            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.CategoryID == id);

            if (hasEventLinks || hasEnrolments)
            {
                return Conflict(
                    "This category cannot be deleted because it has related events or enrolments.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Category deleted successfully.");
        }
    }
}


   

