using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TodoApi.Models;
using TodoApi.Database;
using TodoApi.Business;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private StoreLogic storeLogic = new StoreLogic();

        [HttpPost]
        public IActionResult CreateCart([FromBody] Cart cart)
        {
            var purchaseNumber = storeLogic.Purchase(cart);
            var response = new { purchaseNumber };
            return Ok(response);
        }
    }
}