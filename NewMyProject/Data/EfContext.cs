using Microsoft.EntityFrameworkCore;
using NewMyProject.Entities;

namespace NewMyProject.Data
{
    public class EfContext : DbContext
    {
        public EfContext(DbContextOptions dbContextOptions)
           : base(dbContextOptions)
        {
            
        }
        public DbSet<User> LoginModels { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<TypeProduct> TypeProducts { get; set; }
        public DbSet<WeightProduct> WeightProducts { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(p => p.Weight)
                .WithMany();

            modelBuilder.Entity<Item>()
                .HasOne(p => p.Type)
                .WithMany();

            modelBuilder.Entity<Item>()
                .HasOne(p => p.Product)
                .WithMany();

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Weights)
                .WithOne()
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Types)
                .WithOne()
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<User>()
                .HasOne(p => p.Profile)
                .WithOne(u => u.User)
                .HasForeignKey<Profile>(c => c.UserInfoKey);
        }
    }
}
