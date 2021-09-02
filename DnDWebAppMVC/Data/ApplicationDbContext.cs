using System;
using System.Collections.Generic;
using System.Text;
using DnDWebAppMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DnDWebAppMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

    //    protected override void OnConfiguring(DbContextOptionsBuilder options)
    //=> options.UseSqlServer("Server=.;Database=DealerLeads;Trusted_Connection=True;");
    }
}
