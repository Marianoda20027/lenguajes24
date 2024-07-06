using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using storeapi.Bussisnes;
using Microsoft.AspNetCore.Authorization;
using storeapi.Models;
//PRODUCT1

namespace storeapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly StoreLogic _storeLogic;

        public CartController(StoreLogic storeLogic)
        {
            _storeLogic = storeLogic ?? throw new ArgumentNullException(nameof(storeLogic));
        }

        [HttpPost]
         [AllowAnonymous]
        public async Task<IActionResult> CreateCart([FromBody] Cart cart)
        {
            ValidateCart(cart);

            var sale = await _storeLogic.PurchaseAsync(cart);

            string purchaseNumber = _storeLogic.PurchaseNumber;

            var response = new { purchaseNumberResponse = purchaseNumber };
            return Ok(response);
        }

        private void ValidateCart(Cart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart), "Cart object cannot be null.");
            }

            if (cart.ProductIds == null || cart.ProductIds.Count == 0)
            {
                throw new ArgumentException("Cart must contain at least one product.", nameof(cart.ProductIds));
            }

            if (string.IsNullOrWhiteSpace(cart.Address))
            {
                throw new ArgumentException("Address must be provided.", nameof(cart.Address));
            }
        }
    }
}
