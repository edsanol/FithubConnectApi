using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ContactInformationConfiguration : IEntityTypeConfiguration<ContactInformation>
    {
        public void Configure(EntityTypeBuilder<ContactInformation> builder)
        {
            builder.HasKey(e => e.ContactInformationID).HasName("primary_key contactInformation");

            builder.ToTable("T_CONTACT_INFORMATION");

            builder.Property(e => e.ContactInformationID).HasColumnName("ContactInformationID");
        }
    }
}
