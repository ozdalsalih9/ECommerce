using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace E_Commerce.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }  
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColorImage> ProductColorImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=MUSTIK\SQLEXPRESS; Database=ECommerceDb; Trusted_Connection=True; TrustServerCertificate=True;");

             // ModelValidationWarning yerine ValidationWarning kullanıldı

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.AppUserId, f.ProductId })
                .IsUnique();
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 9, // Eğer Id identity ise dikkat et
                    Name = "Admin",
                    Surname = "User",
                    Email = "admin@admin.com",
                    Password = "AQAAAAEAACcQAAAAEPhLrFcyyYp7mJoZJjCnWnK5z6V8y96v9xv91w+uUuYmndT+Lv8IqydcZcO4Tr9H5A==",
                    Phone = "0000000000",
                    IsActive = true,
                    IsAdmin = true,
                    CreateDate = DateTime.Now,
                    UserGuid = Guid.NewGuid()
                }
            );

            modelBuilder.Entity<Color>().HasData(
                new Color { Id = 1, Name = "Kırmızı", Code = "#FF0000" },
                new Color { Id = 2, Name = "Mavi", Code = "#0000FF" },
                new Color { Id = 3, Name = "Yeşil", Code = "#008000" },
                new Color { Id = 4, Name = "Krem", Code = "#FFFDD0" },
                new Color { Id = 5, Name = "Siyah", Code = "#000000" },
                new Color { Id = 6, Name = "Beyaz", Code = "#FFFFFF" },
                new Color { Id = 7, Name = "Gri", Code = "#808080" },
                new Color { Id = 8, Name = "Lacivert", Code = "#000080" },
                new Color { Id = 9, Name = "Sarı", Code = "#FFFF00" },
                new Color { Id = 10, Name = "Turuncu", Code = "#FFA500" },
                new Color { Id = 11, Name = "Mor", Code = "#800080" },
                new Color { Id = 12, Name = "Pembe", Code = "#FFC0CB" },
                new Color { Id = 13, Name = "Kahverengi", Code = "#A52A2A" },
                new Color { Id = 14, Name = "Bej", Code = "#F5F5DC" },
                new Color { Id = 15, Name = "Füme", Code = "#4B4B4B" },
                new Color { Id = 16, Name = "Antrasit", Code = "#2F2F2F" },
                new Color { Id = 17, Name = "Altın", Code = "#FFD700" },
                new Color { Id = 18, Name = "Gümüş", Code = "#C0C0C0" },
                new Color { Id = 19, Name = "Bordo", Code = "#800000" },
                new Color { Id = 20, Name = "Turkuaz", Code = "#40E0D0" },
                new Color { Id = 21, Name = "Mint", Code = "#98FF98" },
                new Color { Id = 22, Name = "Hardal", Code = "#FFDB58" },
                new Color { Id = 23, Name = "Şarap", Code = "#722F37" },
                new Color { Id = 24, Name = "Koyu Yeşil", Code = "#006400" },
                new Color { Id = 25, Name = "Açık Mavi", Code = "#ADD8E6" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
