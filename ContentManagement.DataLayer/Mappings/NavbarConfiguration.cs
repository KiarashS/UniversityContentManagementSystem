using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentManagement.DataLayer.Mappings
{
    public class NavabarConfiguration : IEntityTypeConfiguration<Navbar>
    {
        public void Configure(EntityTypeBuilder<Navbar> builder)
        {
            builder.Property(x => x.Icon).HasMaxLength(50);

            // Self Referencing Entity
            builder.HasOne(x => x.Parent)
                    .WithMany(x => x.Childrens)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict); // Default DeleteBehavior is ClientSetNull, because relationship is optional.

            builder .HasOne(x => x.Portal)
                    .WithMany(x => x.Navbars)
                    .HasForeignKey(x => x.PortalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
