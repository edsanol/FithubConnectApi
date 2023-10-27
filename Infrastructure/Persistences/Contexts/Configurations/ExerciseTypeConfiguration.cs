using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ExerciseTypeConfiguration : IEntityTypeConfiguration<ExerciseType>
    {
        public void Configure(EntityTypeBuilder<ExerciseType> builder)
        {
            builder.HasKey(e => e.ExerciseTypeId).HasName("primary_key exercise type");

            builder.ToTable("T_EXERCISE_TYPE");

            builder.Property(e => e.ExerciseTypeId).HasColumnName("ExerciseTypeID");
        }
    }
}
