using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class CardAccessConfiguration : IEntityTypeConfiguration<CardAccess>
    {
        public void Configure(EntityTypeBuilder<CardAccess> builder)
        {
            builder.HasKey(e => e.CardId).HasName("primary_key card access");

            builder.ToTable("T_CARD_ACCESS");

            builder.HasIndex(e => e.CardNumber, "card number unique").IsUnique();

            builder.Property(e => e.CardId).HasColumnName("CardID");
            builder.Property(e => e.Status).HasDefaultValueSql("true");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.CardAccesses)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("athlete and card access relation");
        }
    }
}
