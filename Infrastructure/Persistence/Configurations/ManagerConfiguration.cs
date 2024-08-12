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
    public class ManagerConfiguration : IEntityTypeConfiguration<ApplicationManager>
    {
        public void Configure(EntityTypeBuilder<ApplicationManager> builder)
        {
            builder.OwnsOne(manager => manager.paymentInfo);
            builder.HasIndex(a => a.storeName)
                .IsUnique();
        }
    }
}
