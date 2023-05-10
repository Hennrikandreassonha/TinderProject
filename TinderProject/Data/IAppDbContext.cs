using Microsoft.EntityFrameworkCore;
using TinderProject.Models;

namespace TinderProject.Data
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Interests> Interests { get; set; }
        DbSet<Match> Matches { get; set; }
        DbSet<Interaction> Interactions { get; set; }
        DbSet<Message> Messages { get; set; }

        int SaveChanges();
    }
}