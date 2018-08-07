using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class ImageLinkConfiguration : IEntityTypeConfiguration<ImageLink>
    {
        public void Configure(EntityTypeBuilder<ImageLink> builder)
        {
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Imagename).IsRequired();
            builder.Property(x => x.Url).IsRequired();

            builder.HasOne(x => x.Portal)
                    .WithMany(x => x.ImageLinks)
                    .HasForeignKey(x => x.PortalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
