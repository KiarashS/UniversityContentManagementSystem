using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class ContentVideoConfiguration : IEntityTypeConfiguration<ContentVideo>
    {
        public void Configure(EntityTypeBuilder<ContentVideo> builder)
        {
            builder.Property(x => x.Videoname).IsRequired();

            builder.HasOne(x => x.Content)
                    .WithMany(x => x.ContentVideos)
                    .HasForeignKey(x => x.ContentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
