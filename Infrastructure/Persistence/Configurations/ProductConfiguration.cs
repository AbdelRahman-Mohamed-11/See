using Core.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.UseTptMappingStrategy();

            builder
           .HasOne(p => p.Category)
           .WithMany()
           .HasForeignKey(p => p.CategoryId)
            .IsRequired();

            builder
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .IsRequired();

            builder
            .HasOne<ApplicationUser>() // Change this to your ApplicationUser type
            .WithMany()
            .HasForeignKey("ManagerId") // Shadow property for the foreign key
            .IsRequired();

            builder.Property(p => p.Price)
               .HasColumnType("decimal(18,2)");
        }
    }
}
