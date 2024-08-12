using Core.enums;
using Core.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class GlassConfiguration : IEntityTypeConfiguration<Glass>
    {
        public void Configure(EntityTypeBuilder<Glass> builder)
        {
            builder.Property(g => g.FrameSize)
                 .HasConversion(
                     g => g.ToString(),  // store as string in the database
                     g => (FrameSize)Enum.Parse(typeof(FrameSize), g)
                 // convert string to enum when retrieve the value
                 );
        }
    }
}
