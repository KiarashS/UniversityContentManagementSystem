using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class FooterLinkConfiguration : IEntityTypeConfiguration<FooterLink>
    {
        public void Configure(EntityTypeBuilder<FooterLink> builder)
        {
            builder.Property(x => x.Text).IsRequired();
            builder.Property(x => x.Url).IsRequired();

            builder.HasOne(x => x.FooterSection)
                    .WithMany(x => x.Links)
                    .HasForeignKey(x => x.FooterSectionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
