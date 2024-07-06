using Microsoft.AspNetCore.Mvc;
using storeapi.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace storeapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class storeController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetStore()
        {
            // Obtener la instancia de la tienda
            Store storeInstance = Store.Instance;

            if (storeInstance == null || storeInstance.Products == null)
            {
                return NotFound("No se encontraron productos en la tienda.");
            }

            // Obtener todas las categorías
            Categories categories = new Categories();



            List<object> categoryList = new List<object>();

            foreach (Category category in categories.ListCategories)
            {
                categoryList.Add(new { Id = category.Id, Name = category.Name });
            }
            var response = new
            {
                Products = storeInstance.Products,
                Categories = categoryList
            };

            return Ok(response);
        }
    }
}
