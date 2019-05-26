using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class ContentAudioConfiguration : IEntityTypeConfiguration<ContentAudio>
    {
        public void Configure(EntityTypeBuilder<ContentAudio> builder)
        {
            builder.Property(x => x.Audioname).IsRequired();

            builder.HasOne(x => x.Content)
                    .WithMany(x => x.ContentAudios)
                    .HasForeignKey(x => x.ContentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
