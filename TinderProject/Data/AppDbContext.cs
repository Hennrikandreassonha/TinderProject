using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TinderProject.Models;

namespace TinderProject.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Accounts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
