using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AthleteTokenConfiguration : IEntityTypeConfiguration<AthleteToken>
    {
        public void Configure(EntityTypeBuilder<AthleteToken> builder)
        {
            builder.HasKey(e => e.TokenID).HasName("primary_key athlete token");

            builder.ToTable("T_ATHLETE_TOKEN");

            builder.Property(e => e.TokenID).HasColumnName("TokenID");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.AthleteToken)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("athlete and measurements progress relation");
        }
    }
}
