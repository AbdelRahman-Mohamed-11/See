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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.ItemOrdered, i => {
                i.WithOwner();
            });

            builder.Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            builder.HasOne<ApplicationManager>()
                .WithMany()
                .HasForeignKey(o => o.ManagerID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
