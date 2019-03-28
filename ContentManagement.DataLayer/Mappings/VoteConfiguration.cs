using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.Property(e => e.Title).IsRequired();

            builder.HasMany(x => x.Items).WithOne(x => x.Vote).HasForeignKey(x => x.VoteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
