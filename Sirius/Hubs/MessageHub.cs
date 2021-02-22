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


