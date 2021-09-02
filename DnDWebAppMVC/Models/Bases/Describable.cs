using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models.Interfaces;

namespace DnDWebAppMVC.Models.Bases
{
    public class Describable : IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
