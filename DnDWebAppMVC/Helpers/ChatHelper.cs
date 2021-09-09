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
        public List<Character> Characters { get; set; }
        public GameRoom GameRoom { get; set; }

        public ChatHelper()
        {
            Messages = new List<Message>();
            Characters = new List<Character>();
        }

        public List<Message> GetFilteredMessages(Guid playerId, Guid ownerId)
        {
            List<Message> messages = new List<Message>();

            if (playerId != ownerId)
                return Messages.Where(m => !m.IsPrivate || (m.IsPrivate && (m.ReceiverId == playerId || m.SenderId == playerId))).ToList();
            else
                return Messages.ToList();
        }
    }
}
