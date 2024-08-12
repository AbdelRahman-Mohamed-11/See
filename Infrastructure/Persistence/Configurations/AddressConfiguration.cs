using Core.Entities.Delivery;
using Core.Entities.Order;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Addresses) 
                .HasForeignKey(a => a.UserId);
            builder
                .HasOne<City>()
                .WithMany()
                .HasForeignKey(c => c.CityId);
        }
    }
}
