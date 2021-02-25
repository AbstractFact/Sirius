using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sirius.Entities;

namespace Sirius.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string mess)
        {
            await Clients.All.SendAsync("ReceiveMessage", mess);
        }

        public async Task StartReceivingRequests(int receiverID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{receiverID}");
        }

        public async Task StartReceivingRecommendations(string genre)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{genre}");
        }

        public void sendToAll(Message mess)
        {
            Clients.All.SendAsync("sendToAll", mess);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}


