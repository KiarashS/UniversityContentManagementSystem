using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(450).IsRequired();
            builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}
