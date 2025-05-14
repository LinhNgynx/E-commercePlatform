using E_commerceData.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;

namespace E_commerceData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ApplicationDbContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new {
                        id = c.Id,
                        name = c.Name,
                        discount_percentage = c.DiscountPercentage
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetProductsByCategory(int id, [FromQuery] string? size, [FromQuery] string? color)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.CategoryId == id)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Price,
                        Variants = p.Variants
                            .Where(v =>
                                (string.IsNullOrEmpty(color) || v.Color == color) &&
                                (string.IsNullOrEmpty(size) || v.Size == size)
                            )
                            .Select(v => new
                            {
                                v.Id,
                                v.Color,
                                v.Size,
                                v.StockQuantity
                            })
                            .ToList()
                    })
                    .Where(p => p.Variants.Any()) 
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string? q, [FromQuery] string? color, [FromQuery] string? size)
        {
            try
            {
                var productsQuery = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(q))
                {
                    productsQuery = productsQuery.Where(p => p.Name.Contains(q));
                }

                var products = await productsQuery
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Price,
                        Category = p.Category.Name,
                        Variants = p.Variants
                            .Where(v =>
                                (string.IsNullOrEmpty(color) || v.Color == color) &&
                                (string.IsNullOrEmpty(size) || v.Size == size)
                            )
                            .Select(v => new
                            {
                                v.Id,
                                v.Color,
                                v.Size,
                                v.StockQuantity
                            })
                            .ToList()
                    })
                    .Where(p => p.Variants.Any()) 
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching products.", error = ex.Message });
            }
        }

    }

}
