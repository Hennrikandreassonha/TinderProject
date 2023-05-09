using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TinderProject.Models;

namespace TinderProject.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Interests> Interests { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<PersonalType> PersonalTypes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Liker)
                .WithMany(u => u.LikedUsers)
                .HasForeignKey(i => i.LikerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Liked)
                .WithMany(u => u.LikedByUsers)
                .HasForeignKey(i => i.LikedId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Match>()
                .HasOne(i => i.User1)
                .WithMany(u => u.Matcher1)
                .HasForeignKey(i => i.User1Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Match>()
                .HasOne(i => i.User2)
                .WithMany(u => u.Matcher2)
                .HasForeignKey(i => i.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SendTo)
                .WithMany()
                .HasForeignKey(m => m.SentToId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SentFrom)
                .WithMany()
                .HasForeignKey(m => m.SentFromId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}