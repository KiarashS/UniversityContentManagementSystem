using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class ContentGalleryConfiguration : IEntityTypeConfiguration<ContentGallery>
    {
        public void Configure(EntityTypeBuilder<ContentGallery> builder)
        {
            builder.Property(x => x.Imagename).IsRequired();

            builder.HasOne(x => x.Content)
                    .WithMany(x => x.ContentGalleries)
                    .HasForeignKey(x => x.ContentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
