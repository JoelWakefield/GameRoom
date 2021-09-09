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

        public ChatHub(ChatHelper chatHelper)
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

        public async Task ConnectUser(Character character, Guid hostId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId, 
                character.OwnerId.ToString()
            );
            await Clients.Group(character.OwnerId.ToString()).SendAsync(
                "LoadChatRoom", 
                _chatHelper.Characters, 
                _chatHelper.GetFilteredMessages(character.OwnerId, hostId)
            );

            _chatHelper.Characters.Add(character);
            await Clients.All.SendAsync("UserEntered", character);
        }

        public async Task DisconnectUser(Character character)
        {
            _chatHelper.Characters.Remove(character);

            await Clients.All.SendAsync("UserLeft", character);
        }

        public async Task SendPublicMessage(Message message)
        {
            message.Id = Guid.NewGuid();
            message.SentOn = DateTime.Now;

            _chatHelper.Messages.Add(message);

            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendPrivateMessage(Message message)
        {
            message.Id = Guid.NewGuid();
            message.SentOn = DateTime.Now;

            _chatHelper.Messages.Add(message);

            await Clients.Group(message.SenderId.ToString()).SendAsync("ReceiveMessage", message);
            await Clients.Group(message.ReceiverId.ToString()).SendAsync("ReceiveMessage", message);
        }


        //private async Task<UserProfile> GetProfile(Guid id)
        //{
        //    var userId = AuthHelper.GetOid(User);
        //    return await _azureSQL.UserProfiles
        //        .FirstOrDefaultAsync(p => p.Id == id);
        //}
    }
}
