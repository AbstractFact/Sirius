using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sirius.Entities;

namespace Sirius.Hubs
{
    public class MessageHub : Hub
    {
        public async Task StartReceivingRequests(int receiverID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{receiverID}");
        }

        public async Task StartReceivingRecommendations(string genre)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{genre}");
        }

        public async Task StopReceivingRecommendations(string genre)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"channel:{genre}");
        }
    }
}


