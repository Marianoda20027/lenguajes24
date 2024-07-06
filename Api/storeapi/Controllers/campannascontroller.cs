using Microsoft.AspNetCore.Mvc;
using storeapi.Database;
using storeapi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using storeapi.Hubs;

namespace storeapi
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampannasController : ControllerBase
    {
        private readonly IHubContext<CampaignHub> _hubContext;

        public CampannasController(IHubContext<CampaignHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostCampanna([FromBody] Campanna campanna)
        {
            if (campanna == null)
            {
                return BadRequest("Campanna object is null");
            }

            await CampannaDB.InsertCampannaAsync(campanna);
            await _hubContext.Clients.All.SendAsync("UpdateCampaigns", campanna.ContenidoHtml, campanna.Estado);

            return Ok(campanna);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCampannaEstado(int id, [FromBody] Campanna campanna)
        {
            if (campanna == null || id != campanna.Id)
            {
                return BadRequest("Invalid campanna object or ID");
            }

            await CampannaDB.UpdateCampannaEstadoAsync(id, campanna.Estado);
            Campanna updatedCampanna = await CampannaDB.GetCampannaByIdAsync(id);
            await _hubContext.Clients.All.SendAsync("UpdateCampaigns", updatedCampanna.ContenidoHtml, updatedCampanna.Estado);

            return Ok(updatedCampanna);
        }

        [HttpGet]
        public async Task<IActionResult> GetCampannas()
        {
            IEnumerable<Campanna> campannas = await CampannaDB.LoadCampannasFromDatabaseAsync();
            return Ok(campannas);
        }
    }
}
