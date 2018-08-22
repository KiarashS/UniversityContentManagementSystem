﻿using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class PortalConfiguration : IEntityTypeConfiguration<Portal>
    {
        public void Configure(EntityTypeBuilder<Portal> builder)
        {
            builder.Property(x => x.PortalKey).HasMaxLength(400);
            builder.HasIndex(x => x.PortalKey).IsUnique();
            builder.Property(x => x.TitleFa).HasMaxLength(200).IsRequired();
            builder.Property(x => x.HtmlTitleFa).IsRequired();
            builder.Property(x => x.DescriptionFa).HasMaxLength(500).IsRequired();
            builder.Property(x => x.TitleEn).HasMaxLength(200).IsRequired();
            builder.Property(x => x.HtmlTitleEn).IsRequired();
            builder.Property(x => x.DescriptionEn).HasMaxLength(500).IsRequired();
        }
    }
}
