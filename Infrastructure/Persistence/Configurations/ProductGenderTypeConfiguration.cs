using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Infrastructure.Persistence.Configurations
{


    public class ProductGenderTypeConfiguration : IEntityTypeConfiguration<ProductGenderType>
    {
        public void Configure(EntityTypeBuilder<ProductGenderType> builder)
        {
            builder
             .HasKey(pg => new { pg.ProductId, pg.GenderTypeId });

            builder
                .HasOne(pg => pg.Product)
                .WithMany(p => p.ProductGenderTypes)
                .HasForeignKey(pg => pg.ProductId);

            builder
                .HasOne(pg => pg.GenderType)
                .WithMany(gt => gt.ProductGenderTypes)
                .HasForeignKey(pg => pg.GenderTypeId);
        }
    }
}
