using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class LinkConfiguration : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.Property(x => x.Text).IsRequired();
            builder.Property(x => x.Url).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(50);
            builder.Property(x => x.IconColor).HasMaxLength(10);

            builder.HasOne(x => x.Portal)
                    .WithMany(x => x.Links)
                    .HasForeignKey(x => x.PortalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
