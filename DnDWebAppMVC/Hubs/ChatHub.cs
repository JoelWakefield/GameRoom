using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Data;
using DnDWebAppMVC.Helpers;
using DnDWebAppMVC.Models;
using Microsoft.AspNetCore.SignalR;

namespace DnDWebAppMVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatHelper _chatHelper;

        public ChatHub(MessageHelper messageHelper, ChatHelper chatHelper)
        {
            _chatHelper = chatHelper;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task ConnectUser(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, username);
            await Clients.Group(username).SendAsync("LoadChatRoom", _chatHelper.Users, _chatHelper.Messages);
            
            _chatHelper.Users.Add(username);
            await Clients.All.SendAsync("UserEntered", username);
        }

        public async Task DisconnectUser(string username)
        {
            _chatHelper.Users.Remove(username);

            await Clients.All.SendAsync("UserLeft", username);
        }

        public async Task SendPublicMessage(string sender, string message)
        {
            _chatHelper.Messages.Add(new Message()
            {
                SenderName = sender,
                Text = message
            });

            await Clients.All.SendAsync("ReceiveMessage", sender, message);
        }

        public async Task SendPrivateMessage(string receiver, string sender, string message)
        {
            await Clients.Group(sender).SendAsync("ReceiveMessage", sender, message);
            await Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }
    }
}
