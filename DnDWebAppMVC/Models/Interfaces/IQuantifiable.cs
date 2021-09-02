using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Models.Interfaces
{
    interface IQuantifiable
    {
        public string Name { get; set; }
        public byte Value { get; set; }
    }
}
