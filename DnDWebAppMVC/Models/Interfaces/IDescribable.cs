using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Models.Interfaces
{
    interface IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
