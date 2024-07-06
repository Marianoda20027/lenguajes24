using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using storeapi.Models;
using storeapi.Business;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using storeapi.Database;
using System.ComponentModel.DataAnnotations;

namespace storeapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsertProductsController : ControllerBase
    {
        private readonly InsertProductsLogic _insertProductsLogic;
        private readonly Categories _categories;

        public InsertProductsController(IMemoryCache cache, Categories categories)
        {
            _categories = categories;
            _insertProductsLogic = new InsertProductsLogic(cache, StoreDB.InsertProduct);
        }

        [HttpPost]
        public IActionResult InsertProduct([FromBody] ProductRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);
            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                return BadRequest(validationResults);
            }

            Category category = _categories.GetCategoryById(request.CategoryId);
        

            Product product = new Product
            {
                id = request.Id,
                Name = request.Name,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                Description = request.Description,
                Category = category
            };

            var updatedProducts = _insertProductsLogic.InsertProduct(product);
            return Ok(updatedProducts);
        }
    }

    public class ProductRequestModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}


