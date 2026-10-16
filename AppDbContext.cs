
using Microsoft.EntityFrameworkCore;
using RaceDayApi1.Models;

namespace RaceDayApi1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Enrolments> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users primary key
            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserID);

            // Events primary key
            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventID);

            // Categories primary key
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryID);

            // EventCategories composite primary key
            modelBuilder.Entity<EventCategory>()
                .HasKey(ec => new { ec.EventID, ec.CategoryID });

            // Enrolments primary key
            modelBuilder.Entity<Enrolments>()
                .HasKey(e => e.EnrolmentID);

            // Results primary key
            modelBuilder.Entity<Result>()
                .HasKey(r => r.ResultID);
        }
    }
}

