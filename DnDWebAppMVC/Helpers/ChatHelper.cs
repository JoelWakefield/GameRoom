using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models;

namespace DnDWebAppMVC.Helpers
{
    public class ChatHelper
    {
        public List<Message> Messages { get; set; }
        public List<string> Users { get; set; }

        public ChatHelper()
        {
            Messages = new List<Message>();
            Users = new List<string>();
        }

        public List<Message> GetPrivateMessages(Guid playerId, Guid ownerId)
        {
            if (playerId != ownerId)
                return Messages
                    .Where(m => (m.IsPrivate && m.SenderId == ownerId) || !m.IsPrivate)
                    .ToList();
            else
                return Messages.ToList();
        }
    }
}
