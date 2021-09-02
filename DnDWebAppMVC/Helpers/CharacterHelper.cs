using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models;

namespace DnDWebAppMVC.Data
{
    public class CharacterHelper
    {
        private List<Character> Characters;

        public CharacterHelper()
        {
            Characters = new List<Character>();
        }

        public List<Character> Get()
        {
            return Characters;
        }

        public void Set(Character character)
        {
            Characters.Add(character);
        }
    }
}
