using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    internal class PaymentsConfiguration : IEntityTypeConfiguration<Payments>
    {
        public void Configure(EntityTypeBuilder<Payments> builder)
        {
            builder.HasKey(e => e.PaymentId).HasName("t_payments_pkey");

            builder.ToTable("T_PAYMENTS");

            builder.Property(e => e.PaymentId).HasColumnName("PaymentID");

            builder.Property(e => e.AmountPaid).HasColumnType("numeric");

            builder.Property(e => e.Notes).HasColumnType("text");

            builder.Property(e => e.PaymentDate).HasColumnType("timestamp without time zone");

            builder.Property(e => e.PaymentDate).HasDefaultValue(DateTime.Now);

            builder.Property(e => e.PaymentMethod).IsRequired();

            builder.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_payments_orderid_fkey");
        }
    }
}
