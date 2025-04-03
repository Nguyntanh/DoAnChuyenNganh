using Microsoft.EntityFrameworkCore;
using DoAnData.Models;
using DoAnData.Models;

namespace DoAnData.Data
{
    public class DoAnDataContext : DbContext
    {
        public DoAnDataContext(DbContextOptions<DoAnDataContext> options) : base(options) { }

        // DbSet cho các bảng
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cấu hình bảng Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // 2. Cấu hình bảng Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasIndex(u => u.Username)
                      .IsUnique();
                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(256);
                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.HasIndex(u => u.Email)
                      .IsUnique();
                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasConversion<string>(); // Chuyển enum thành string
                entity.Property(u => u.Status)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasConversion<string>();
                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            // 3. Cấu hình bảng Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(p => p.Stock)
                      .IsRequired();
                entity.Property(p => p.Status)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                // Quan hệ với Users (Seller) và Categories
                entity.HasOne(p => p.Seller)
                      .WithMany()
                      .HasForeignKey(p => p.SellerId)
                      .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade để bảo vệ dữ liệu
                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Cấu hình bảng Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.TotalPrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(o => o.Status)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(o => o.OrderDate)
                      .HasDefaultValueSql("GETDATE()");
                entity.Property(o => o.ShippingAddress)
                      .IsRequired()
                      .HasMaxLength(255);

                // Quan hệ với Users (Customer và Seller)
                entity.HasOne(o => o.Customer)
                      .WithMany()
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(o => o.Seller)
                      .WithMany()
                      .HasForeignKey(o => o.SellerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 5. Cấu hình bảng OrderDetails
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => new { od.OrderId, od.ProductId });
                entity.Property(od => od.Quantity)
                      .IsRequired();
                entity.Property(od => od.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                // Quan hệ với Orders và Products
                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Product)
                      .WithMany()
                      .HasForeignKey(od => od.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 6. Cấu hình bảng Reviews
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Rating)
                      .IsRequired();
                entity.Property(r => r.ReviewDate)
                      .HasDefaultValueSql("GETDATE()");

                // Quan hệ với Users (Customer) và Products
                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Product)
                      .WithMany()
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 7. Cấu hình bảng Cart
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                // Quan hệ với Users (Customer)
                entity.HasOne(c => c.Customer)
                      .WithMany()
                      .HasForeignKey(c => c.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 8. Cấu hình bảng CartItem
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => new { ci.CartId, ci.ProductId });
                entity.Property(ci => ci.Quantity)
                      .IsRequired();

                // Quan hệ với Cart và Products
                entity.HasOne(ci => ci.Cart)
                      .WithMany(c => c.CartItems)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ci => ci.Product)
                      .WithMany()
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 9. Cấu hình bảng SupportTicket
            modelBuilder.Entity<SupportTicket>(entity =>
            {
                entity.HasKey(st => st.Id);
                entity.Property(st => st.Title)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(st => st.Description)
                      .IsRequired();
                entity.Property(st => st.Status)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasDefaultValue("open");
                entity.Property(st => st.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                // Quan hệ với Users và Orders
                entity.HasOne(st => st.User)
                      .WithMany()
                      .HasForeignKey(st => st.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(st => st.Order)
                      .WithMany()
                      .HasForeignKey(st => st.OrderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}