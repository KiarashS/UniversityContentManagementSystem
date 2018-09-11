using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class FooterSectionConfiguration : IEntityTypeConfiguration<FooterSection>
    {
        public void Configure(EntityTypeBuilder<FooterSection> builder)
        {
            builder.Property(x => x.Title).IsRequired();

            builder.HasOne(x => x.Portal)
                    .WithMany(x => x.FooterSections)
                    .HasForeignKey(x => x.PortalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
