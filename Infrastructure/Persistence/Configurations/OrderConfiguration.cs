using Core.Entities.Order;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
          

            builder.Property(o => o.Status)
                .HasConversion( 
                    o => o.ToString(),  // store as string in the database
                    o => (OrderStatus) Enum.Parse(typeof(OrderStatus),o)   
                    // convert string to enum when retrieve the value
                );

            


            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder
               .HasOne<ApplicationUser>()
               .WithMany(u => u.Orders)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Restrict); // Set OnDelete to Restrict

        }
    }
}
