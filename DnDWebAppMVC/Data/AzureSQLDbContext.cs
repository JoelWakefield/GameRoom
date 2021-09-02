using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnDWebAppMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DnDWebAppMVC.Data
{
    public class AzureSQLDbContext : DbContext
    {
        public AzureSQLDbContext(DbContextOptions<AzureSQLDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
