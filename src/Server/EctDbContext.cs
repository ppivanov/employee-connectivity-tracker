using Microsoft.EntityFrameworkCore;
using EctBlazorApp.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using EctBlazorApp.Server.CommonMethods;
using EctBlazorApp.Shared.GraphModels;
using System.Collections.Generic;

namespace EctBlazorApp.Server
{
    public class EctDbContext : DbContext
    {
        public DbSet<EctTeam> Teams { get; set; }
        public DbSet<EctUser> Users { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<ReceivedMail> ReceivedEmails { get; set; }
        public DbSet<SentMail> SentEmails { get; set; }


        public EctDbContext()
        {
        }

        public EctDbContext(DbContextOptions<EctDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEvent>().Ignore(e => e.Attendees);
            modelBuilder.Entity<SentMail>().Ignore(e => e.Recipients);

            modelBuilder.Entity<EctTeam>()
                .HasOne(p => p.Leader)
                .WithMany(p => p.LeaderOf);
        }

        public async Task<EctUser> AddUser(string userId, HttpClient client)
        {
            GraphUserResponse graphUser = await client.GetGraphUser(userId);
            try
            {
                EctUser newUser = new EctUser(graphUser);
                Users.Add(newUser);
                await SaveChangesAsync();
                return newUser;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(EctDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any())
            {
                return;
            }
        }
    }
}
