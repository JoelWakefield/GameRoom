using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Models.Interfaces
{
    interface IActionable
    {
        public bool IsBonus { get; set; }
        public string Use();
    }
}
