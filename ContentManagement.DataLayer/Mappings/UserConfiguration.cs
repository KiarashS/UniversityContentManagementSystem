using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(e => e.Username).HasMaxLength(450).IsRequired();
            builder.HasIndex(e => e.Username).IsUnique();
            builder.Property(e => e.Email).IsRequired();
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Password).IsRequired();
            builder.Property(e => e.SerialNumber).HasMaxLength(450);
        }
    }
}
