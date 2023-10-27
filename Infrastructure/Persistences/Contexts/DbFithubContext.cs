using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistences.Contexts;

public partial class DbFithubContext : DbContext
{
    public DbFithubContext()
    {
    }

    public DbFithubContext(DbContextOptions<DbFithubContext> options)
        : base(options)
    {
    }

    static DbFithubContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public virtual DbSet<AccessLog> AccessLog { get; set; }

    public virtual DbSet<Athlete> Athlete { get; set; }

    public virtual DbSet<AthleteProgress> AthleteProgress { get; set; }

    public virtual DbSet<CardAccess> CardAccess { get; set; }

    public virtual DbSet<ExerciseMetric> ExerciseMetric { get; set; }

    public virtual DbSet<ExerciseType> ExerciseType { get; set; }

    public virtual DbSet<Gym> Gym { get; set; }

    public virtual DbSet<MeasurementsProgress> MeasurementsProgress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
