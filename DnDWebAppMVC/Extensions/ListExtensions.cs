using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Extensions
{
    public static class ListExtensions
    {
        public static void Set<T>(this List<T> list, T item)
        {
            list.Add(item);
        }
    }
}
