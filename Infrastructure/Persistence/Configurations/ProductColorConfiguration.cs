using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
    {
        public void Configure(EntityTypeBuilder<ProductColor> builder)
        {
            builder
           .HasKey(pg => new { pg.ProductId, pg.ColorId });

            builder
                .HasOne(pg => pg.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pg => pg.ProductId);

            builder
                .HasOne(pg => pg.Color)
                .WithMany(gt => gt.ProductColors)
                .HasForeignKey(pg => pg.ColorId);
        }
    }
}
