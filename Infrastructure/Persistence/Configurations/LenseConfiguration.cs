using Core.Products;
using Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class LenseConfiguration : IEntityTypeConfiguration<Lense>
    {
        public void Configure(EntityTypeBuilder<Lense> builder)
        {
            builder.Property(l => l.LensUsage)
                .HasConversion(
                    l => l.ToString(),  // store as string in the database
                    l => (LensUsage)Enum.Parse(typeof(LensUsage), l)
                // convert string to enum when retrieve the value
                );

            
        }
    }
}
