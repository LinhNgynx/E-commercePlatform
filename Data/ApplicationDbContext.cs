using E_commerceData.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerceData.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<OrderVoucher> OrderVouchers => Set<OrderVoucher>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Phone).IsRequired().HasMaxLength(20);
            });

            // Address
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Province).IsRequired().HasMaxLength(50);
                entity.Property(a => a.District).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Commune).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Detail).IsRequired().HasMaxLength(200);
                entity.Property(a => a.HousingType).IsRequired().HasMaxLength(50);

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Addresses)
                      .HasForeignKey(a => a.UserId);
            });

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.Property(c => c.DiscountPercentage).HasColumnType("decimal(5,2)");
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(10,2)");

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId);
            });

            // ProductVariant
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasKey(pv => pv.Id);
                entity.Property(pv => pv.Color).IsRequired().HasMaxLength(50);
                entity.Property(pv => pv.Size).IsRequired().HasMaxLength(20);

                entity.HasOne(pv => pv.Product)
                      .WithMany(p => p.Variants)
                      .HasForeignKey(pv => pv.ProductId);

                entity.HasOne(pv => pv.Store)
                      .WithMany(s => s.ProductVariants)
                      .HasForeignKey(pv => pv.StoreId);
            });

            // Store
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Location).IsRequired().HasMaxLength(100);
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OrderDate).IsRequired();
                entity.Property(o => o.TotalPrice).HasColumnType("decimal(10,2)");
                entity.Property(o => o.PaymentStatus).IsRequired().HasMaxLength(20);

                // Prevent cascading delete for User -> Orders
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.NoAction

                // Prevent cascading delete for Address -> Orders
                entity.HasOne(o => o.Address)
                      .WithMany(a => a.Orders)
                      .HasForeignKey(o => o.AddressId)
                      .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.NoAction
            });


            // OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Quantity).IsRequired();
                entity.Property(oi => oi.PriceAtOrder).HasColumnType("decimal(10,2)");

                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId);

                entity.HasOne(oi => oi.ProductVariant)
                      .WithMany(pv => pv.OrderItems)
                      .HasForeignKey(oi => oi.ProductVariantId);
            });

            // Voucher
            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Code).IsRequired().HasMaxLength(50);
                entity.Property(v => v.DiscountAmount).HasColumnType("decimal(10,2)");
            });

            // OrderVoucher
            modelBuilder.Entity<OrderVoucher>(entity =>
            {
                entity.HasKey(ov => ov.Id);

                entity.HasOne(ov => ov.Order)
                      .WithMany(o => o.OrderVouchers)
                      .HasForeignKey(ov => ov.OrderId);

                entity.HasOne(ov => ov.Voucher)
                      .WithMany(v => v.OrderVouchers)
                      .HasForeignKey(ov => ov.VoucherId);
            });
        }
    }
}