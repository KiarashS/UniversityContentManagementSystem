using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Keywords).IsRequired();

            builder.HasOne(x => x.Portal)
                    .WithMany(x => x.Contents)
                    .HasForeignKey(x => x.PortalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
