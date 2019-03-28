using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public class VoteItemConfiguration : IEntityTypeConfiguration<VoteItem>
    {
        public void Configure(EntityTypeBuilder<VoteItem> builder)
        {
            builder.Property(e => e.ItemTitle).IsRequired();

            builder.HasOne(x => x.Vote).WithMany(x => x.Items).HasForeignKey(x => x.VoteId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
