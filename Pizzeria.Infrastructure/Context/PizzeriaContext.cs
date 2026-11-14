using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain.Entities;

namespace Pizzeria.Infrastructure.Context
{
    public class PizzeriaContext : DbContext
    {
        public PizzeriaContext(DbContextOptions<PizzeriaContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();
        public DbSet<ProductIngredient> ProductIngredients => Set<ProductIngredient>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("Products");
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(100);
                e.Property(p => p.Description).HasMaxLength(300);
            });

            modelBuilder.Entity<Size>(e =>
            {
                e.ToTable("Sizes");
                e.HasKey(s => s.Id);
                e.Property(s => s.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<ProductPrice>(e =>
            {
                e.ToTable("ProductPrices");
                e.HasKey(pp => pp.Id);
                e.Property(pp => pp.Price).HasColumnType("decimal(10,2)");
                e.HasOne(pp => pp.Product)
                    .WithMany(p => p.Prices)
                    .HasForeignKey(pp => pp.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(pp => pp.Size)
                    .WithMany(s => s.Prices)
                    .HasForeignKey(pp => pp.SizeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ingredient>(e =>
            {
                e.ToTable("Ingredients");
                e.HasKey(i => i.Id);
                e.Property(i => i.Name).IsRequired().HasMaxLength(100);
                e.Property(i => i.ExtraPrice).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<ProductIngredient>(e =>
            {
                e.ToTable("ProductIngredients");
                e.HasKey(pi => pi.Id);
                e.HasOne(pi => pi.Product)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(pi => pi.Ingredient)
                    .WithMany(i => i.Products)
                    .HasForeignKey(pi => pi.IngredientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("Customers");
                e.HasKey(c => c.Id);
                e.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
                e.Property(c => c.LastName).IsRequired().HasMaxLength(100);
                e.Property(c => c.Phone).IsRequired().HasMaxLength(20);
                e.Property(c => c.Email).HasMaxLength(150);
                e.Property(c => c.Address).HasMaxLength(300);
                e.Property(c => c.AddressReference).HasMaxLength(300);
                e.Ignore(c => c.FullName);
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(o => o.Id);
                e.Property(o => o.Subtotal).HasColumnType("decimal(12,2)");
                e.Property(o => o.DeliveryFee).HasColumnType("decimal(12,2)");
                e.Property(o => o.Tax).HasColumnType("decimal(12,2)");
                e.Property(o => o.Total).HasColumnType("decimal(12,2)");
                e.Property(o => o.Notes).HasMaxLength(500);
                e.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.ToTable("OrderDetails");
                e.HasKey(d => d.Id);
                e.Property(d => d.UnitPrice).HasColumnType("decimal(10,2)");
                e.Property(d => d.Amount).HasColumnType("decimal(12,2)");
                e.HasOne(d => d.Order)
                    .WithMany(o => o.Details)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(d => d.Size)
                    .WithMany()
                    .HasForeignKey(d => d.SizeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Delivery>(e =>
            {
                e.ToTable("Deliveries");
                e.HasKey(d => d.Id);
                e.Property(d => d.Address).IsRequired().HasMaxLength(300);
                e.Property(d => d.Courier).HasMaxLength(100);
                e.HasOne(d => d.Order)
                    .WithOne(o => o.Delivery)
                    .HasForeignKey<Delivery>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
