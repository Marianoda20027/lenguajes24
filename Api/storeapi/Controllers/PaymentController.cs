using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using storeapi.Models;
using storeapi.Database;
using System.Collections.Generic;

namespace storeapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("payment-methods")]
        public IActionResult GetPaymentMethods()
        {
            var methods = LoadPaymentMethodsFromDatabase();

            if (methods == null || methods.Count == 0)
            {
                return StatusCode(500, "No se encontraron métodos de pago.");
            }

            return Ok(new
            {
                Cash = new { paymentType = PaymentMethods.Type.CASH, isActive = methods.ContainsKey(PaymentMethods.Type.CASH) ? methods[PaymentMethods.Type.CASH].IsActive : false },
                Sinpe = new { paymentType = PaymentMethods.Type.SINPE, isActive = methods.ContainsKey(PaymentMethods.Type.SINPE) ? methods[PaymentMethods.Type.SINPE].IsActive : false }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("update-status")]
        public IActionResult UpdatePaymentStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PaymentMethods.Type paymentType;
            if (!Enum.TryParse(request.Method.ToUpper(), out paymentType))
            {
                return BadRequest("Método de pago no válido.");
            }

            var paymentMethod = PaymentMethods.Find(paymentType);
            if (paymentMethod == null)
            {
                return BadRequest("Método de pago no encontrado.");
            }

            bool isActive = request.State.Equals("activo", StringComparison.OrdinalIgnoreCase);
            paymentMethod.IsActive = isActive;
            PaymentDB.UpdatePaymentMethodStatus((int)paymentType, isActive);

            return Ok(new { message = "Estado del método de pago actualizado con éxito." });
        }

        private Dictionary<PaymentMethods.Type, PaymentMethods> LoadPaymentMethodsFromDatabase()
        {
            var paymentMethods = new Dictionary<PaymentMethods.Type, PaymentMethods>();
            var data = PaymentDB.RetrievePaymentMethods();

            foreach (var methodData in data)
            {
                int id = int.Parse(methodData[0]);
                string name = methodData[1];
                bool isActive = methodData[2] == "true"; // Asegurarse de que el valor se convierta a booleano

                var method = PaymentMethods.LoadFromDatabase(id, name, isActive);
                paymentMethods.Add((PaymentMethods.Type)id, method);
            }

            return paymentMethods;
        }
    }

    public class UpdatePaymentStatusRequest
    {
        public string Method { get; set; }
        public string State { get; set; }
    }
}

