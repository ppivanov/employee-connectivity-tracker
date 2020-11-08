using Microsoft.EntityFrameworkCore;
using EctBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public EctDbContext()
        {
        }

        public EctDbContext(DbContextOptions<EctDbContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //        //optionsBuilder.UseSqlServer("Server=tcp:ect.database.windows.net,1433;Initial Catalog=EctDb;Persist Security Info=False;User ID=geologistsnooze;Password={PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEvent>().Ignore(e => e.Attendees);
            modelBuilder.Entity<SentMail>().Ignore(e => e.Recipients);

            modelBuilder.Entity<EctTeam>()
                .HasOne(p => p.Leader)
                .WithMany(p => p.LeaderOf);
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(EctDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
        }
    }
}
