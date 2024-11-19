using Meshwark.Models;
using Microsoft.EntityFrameworkCore;
using Meshwark.Models;

namespace Meshwark.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Trip> Trips { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
 
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Trips)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
            modelBuilder.Entity<Trip>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Wallet>()
               .HasKey(w => w.UserId);  // Assuming Wallet is tied to a specific User (1-to-1)

            modelBuilder.Entity<Wallet>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Wallet>(w => w.UserId);

            // Configure Transaction
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.Transactions)
                .WithOne()
                .HasForeignKey(t => t.Id);

            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
