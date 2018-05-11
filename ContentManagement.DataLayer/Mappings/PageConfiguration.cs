using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class PageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Text).IsRequired();
            builder.Property(x => x.RawText).IsRequired();
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            builder.HasIndex(x => x.Slug).IsUnique();

            builder.HasOne(x => x.Portal)
                    .WithMany(x => x.Pages)
                    .HasForeignKey(x => x.PortalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
