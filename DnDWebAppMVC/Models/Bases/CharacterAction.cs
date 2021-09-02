using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models.Interfaces;

namespace DnDWebAppMVC.Models.Bases
{
    public class CharacterAction : IActionable, IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBonus { get; set; }

        public string Use()
        {
            return $"{Name} {(IsBonus ? "[Bonus]" : "")} - {Description}";
        }
    }
}
