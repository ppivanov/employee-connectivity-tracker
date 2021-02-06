using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared.Entities;
using EctBlazorApp.Shared.GraphModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EctBlazorApp.Server
{
    public class EctDbContext : DbContext
    {
        public DbSet<EctTeam> Teams { get; set; }
        public DbSet<EctUser> Users { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<ReceivedMail> ReceivedEmails { get; set; }
        public DbSet<SentMail> SentEmails { get; set; }
        public DbSet<EctAdmin> Administrators { get; set; }
        public DbSet<CommunicationPercentage> CommunicationPercentages { get; set; }


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
        }
    }
}
