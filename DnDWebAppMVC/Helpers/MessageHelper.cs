using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models;

namespace DnDWebAppMVC.Data
{
    public class MessageHelper
    {
        private List<Message> Messages;

        public MessageHelper()
        {
            Messages = new List<Message>();
        }

        public List<Message> Get()
        {
            return Messages;
        }

        public List<Message> Get(Guid playerId, Guid ownerId)
        {
            if (playerId != ownerId)
                return Get()
                    .Where(m => (m.IsPrivate && m.SenderId == ownerId) || !m.IsPrivate)
                    .ToList();
            else
                return Get().ToList();
        } 

        public void Set(Message message)
        {
            Messages.Add(message);
        }
    }
}
