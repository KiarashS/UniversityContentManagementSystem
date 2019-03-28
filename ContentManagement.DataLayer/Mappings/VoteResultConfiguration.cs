using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.DataLayer.Mappings
{
    public class VoteResultConfiguration : IEntityTypeConfiguration<VoteResult>
    {
        public void Configure(EntityTypeBuilder<VoteResult> builder)
        {
            //builder.HasKey(e => new { e.VoteId, e.VoteItemId });
            //builder.HasIndex(e => e.VoteId);
            //builder.HasIndex(e => e.VoteItemId);
            //builder.Property(e => e.VoteId);
            //builder.Property(e => e.VoteItemId);
            builder.HasOne(d => d.Vote).WithMany(p => p.VoteResults).HasForeignKey(d => d.VoteId);
            builder.HasOne(d => d.VoteItem).WithMany(p => p.VoteItemResults).HasForeignKey(d => d.VoteItemId);
        }
    }
}
