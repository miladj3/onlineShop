using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using onlineShop.Models;
using onlineShop.Models.ProductModels;

namespace onlineShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDescriptionItem> ProductDescriptionItems { get; set; }
        public DbSet<ProductDescriptionField> ProductDescriptionFields { get; set; }
        public DbSet<ProductAvailabilityNotification> ProductAvalabilityNotifications { get; set; }

        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryDetails> DeliveryDetails { get; set; }

        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<FilePath> FilePaths { get; set; }

        public DbSet<ChangeLog> ChangeLogs { get; set; }

        public DbSet<OrderChangeLog> OrderChangeLogs { get; set; }
        public DbSet<ProductChangeLog> ProductChangeLogs { get; set; }
        public DbSet<UserChangeLog> UserChangeLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ApplyEntityConfigurations(builder);
            base.OnModelCreating(builder);
        }

        private void ApplyEntityConfigurations(ModelBuilder builder)
        {
            // Below Fluent API configurations are kept together under one method 
            // to avoid creation and import of ~10 config files for each entity separately.

            // Order - User
            builder.Entity<Order>()
                .HasOne<AppUser>(a => a.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId);

            builder.Entity<AppUser>()
                 .HasMany<Order>(u => u.Orders)
                 .WithOne(a => a.Customer);

            // Order/Blog/Product - Last Modified By (AppUser)
            builder.Entity<Order>()
                .HasOne<AppUser>(o => o.LastModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.LastModifiedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Blog>()
                .HasOne<AppUser>(o => o.LastModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.LastModifiedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Product>()
                .HasOne<AppUser>(o => o.LastModifiedBy)
                .WithMany()
                .HasForeignKey(o => o.LastModifiedById)
                .OnDelete(DeleteBehavior.SetNull);

            // Product - Description Items
            builder.Entity<Product>()
                .HasMany(p => p.ProductDescriptionItems)
                .WithOne(d => d.Product);

            builder.Entity<Product>()
                .HasMany(p => p.Pictures)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductDescriptionItem>()
                .HasOne(d => d.Product)
                .WithMany(p => p.ProductDescriptionItems);

            builder.Entity<ProductDescriptionItem>()
                .HasKey(d => d.Id);

            // Subcategory - Description Fields - Description Item
            builder.Entity<Subcategory>()
                .HasMany(s => s.DescriptionFields)
                .WithOne(f => f.Subcategory)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProductDescriptionField>()
                .HasOne(f => f.Subcategory)
                .WithMany(s => s.DescriptionFields)
                .HasForeignKey(f => f.SubcategoryId);

            builder.Entity<ProductDescriptionItem>()
                .HasOne(d => d.Field)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser - Customer Address
            builder.Entity<CustomerAddress>()
                .HasOne(a => a.Customer)
                .WithOne(c => c.CustomerAddress);

            builder.Entity<AppUser>()
                .HasOne(u => u.CustomerAddress)
                .WithOne(a => a.Customer);

            builder.Entity<CustomerAddress>()
                 .HasKey(a => a.CustomerId);

            // Order - Delivery Details
            builder.Entity<Order>()
                .HasOne(o => o.DeliveryDetails)
                .WithOne();

            builder.Entity<DeliveryDetails>()
                .HasKey(o => o.OrderId);

            // Delivery Details - Courier Data
            builder.Entity<DeliveryDetails>()
                .HasOne(o => o.CourierData)
                .WithOne()
                .HasForeignKey<CourierData>(d => d.OrderId);

            builder.Entity<CourierData>()
                .HasKey(d => d.OrderId);

            // Delivery Details - Locker Data
            builder.Entity<DeliveryDetails>()
                .HasOne(o => o.LockerData)
                .WithOne()
                .HasForeignKey<LockerData>(d => d.OrderId);

            builder.Entity<LockerData>()
                .HasKey(d => d.OrderId);

            // Delivery Details - Store Data
            builder.Entity<DeliveryDetails>()
                .HasOne(o => o.StoreData)
                .WithOne()
                .HasForeignKey<StoreData>(d => d.OrderId);

            builder.Entity<StoreData>()
                .HasKey(d => d.OrderId);

            // Change Logs for Order, Product & User
            builder.Entity<OrderChangeLog>()
                .HasKey(x => new { x.OrderId, x.ChangeLogId });

            builder.Entity<ProductChangeLog>()
                .HasKey(x => new { x.ProductId, x.ChangeLogId });

            builder.Entity<UserChangeLog>()
                .HasKey(ucl => new { ucl.UserId, ucl.ChangeLogId });

            // Product Availability Notifications
            builder.Entity<ProductAvailabilityNotification>()
                .HasKey(p => p.Id);
        }
    }
}
