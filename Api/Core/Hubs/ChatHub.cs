using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using storeapi.Database;
using storeapi.Models;

//HOLA MUNDO

namespace storeapi.Hubs
{
    public class CampaignHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task NewCampaign(string contenidoHtml)
        {
            // Insert the campaign into the database
            Campanna newCampanna = new Campanna { ContenidoHtml = contenidoHtml, Estado = true };
            CampannaDB.InsertCampannaAsync(newCampanna);
            
            // Notify all clients about the new campaign
            await Clients.All.SendAsync("UpdateCampaigns", newCampanna.ContenidoHtml, newCampanna.Estado);
        }

       public async Task UpdateCampaignState(int id, bool estado)
{
    // Update the campaign state in the database
    await CampannaDB.UpdateCampannaEstadoAsync(id, estado);

    // Retrieve the updated campaign
    Campanna updatedCampanna = await CampannaDB.GetCampannaByIdAsync(id);

    // Notify all clients about the campaign state update
    await Clients.All.SendAsync("UpdateCampaigns", updatedCampanna.ContenidoHtml, updatedCampanna.Estado);
}
    }
}
