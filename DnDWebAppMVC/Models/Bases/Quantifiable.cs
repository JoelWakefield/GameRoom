using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models.Interfaces;

namespace DnDWebAppMVC.Models.Bases
{
    public class Quantifiable : IQuantifiable
    {
        public string Name { get; set; }
        public byte Value { get; set; } = 10;
    }
}
