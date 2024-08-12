using Core.Entities;
using Core.Entities.Delivery;
using Core.Entities.Order;
using Core.Entities.Prescription_Lenses;
using Core.Entities.VisionTest;
using Core.Products;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Product> Products { get; set; }
        
        public DbSet<GenderType> GenderTypes { get; set; }  

        public DbSet<Glass> Glasses { get; set; }

        public DbSet<Lense> Lenses { get; set; }

        public DbSet<Accessory> Accessories { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<FrameType> FrameTypes { get; set; }

        public DbSet<Shape> Shapes { get; set; }

        public DbSet<ProductGenderType> ProductGenderTypes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Address> Address { get; set; } 

        public DbSet<Review> Reviews { get; set; }

        public DbSet<ApplicationManager> Managers { get; set; }

        public DbSet<ApplicationAdmin> Admins { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<PhoneNumber> PhoneNumbers { get; set; }

        public DbSet<DeliveryCostDetail> DeliveryCostDetails { get; set; }

        public DbSet<City> Cities { get; set; }
        
        public DbSet<DeliveryCostPlanSetup> DeliveryCostPlanSetups { get; set; }

        public DbSet<CoatingType> CoatingTypes { get; set; }

        public DbSet<LensType> LensType { get; set; }

        public DbSet<PrescriptionType> PrescriptionTypes { get; set; }

        public DbSet<PriceRange> PriceRanges { get; set; }

        public DbSet<UserPrescription> UserPrescriptions { get; set; }

        public DbSet<VendorCountry> VendorCountries { get; set; }

        public DbSet<PaymobTransaction> PaymobTransactions { get; set; }

        public DbSet<UserFavoriteProduct> UserFavorites { get; set; }

        public DbSet<VisionTest> VisionTests { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) // responsible for create the migration
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Product>()
            .Property(e => e.Id)
            .ValueGeneratedNever(); 

   

            modelBuilder.Entity<UserPrescription>()
                .HasOne(up => up.LensType)
                .WithMany()
                .HasForeignKey(up => up.LensTypeID);

            modelBuilder.Entity<UserPrescription>()
                .HasOne(up => up.CoatingType)
                .WithMany()
                .HasForeignKey(up => up.CoatingTypeID);

            modelBuilder.Entity<UserPrescription>()
                .HasOne(up => up.Country)
                .WithMany()
                .HasForeignKey(up => up.VendorCountryID);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Prescriptions)
                .WithOne()
                .HasForeignKey(x => x.UserID);


        }
    }
}
