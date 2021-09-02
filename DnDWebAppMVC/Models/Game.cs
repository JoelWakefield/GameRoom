using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Models
{
    public class Game
    {
        public GameRoom Room { get; set; }
        public Character PlayerCharacter { get; set; }
        public Message CurrentMessage { get; set; }
        public List<Message> Messages { get; set; }
        public List<Character> Characters { get; set; }
    }
}
