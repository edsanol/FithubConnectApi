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

    public virtual DbSet<AthleteMembership> AthleteMemberships { get; set; }

    public virtual DbSet<AthleteProgress> AthleteProgress { get; set; }

    public virtual DbSet<CardAccess> CardAccess { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<ExerciseMetric> ExerciseMetric { get; set; }

    public virtual DbSet<ExerciseType> ExerciseType { get; set; }

    public virtual DbSet<Gym> Gym { get; set; }

    public virtual DbSet<MeasurementsProgress> MeasurementsProgress { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<ContactInformation> ContactInformation { get; set; }

    public virtual DbSet<AthleteToken> AthleteToken { get; set; }

    public virtual DbSet<GymToken> GymToken { get; set; }

    public virtual DbSet<ProductsCategory> ProductsCategory { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<ProductsVariant> ProductsVariant { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    public virtual DbSet<StockMovements> StockMovements { get; set; }

    public virtual DbSet<Payments> Payments { get; set; }

    public virtual DbSet<AccessType> AccessType { get; set; }

    public virtual DbSet<GymAccessType> GymAccessTypes { get; set; }

    public virtual DbSet<Channels> Channels { get; set; }

    public virtual DbSet<ChannelUsers> ChannelUsers { get; set; }

    public virtual DbSet<Notifications> Notifications { get; set; }

    public virtual DbSet<UserDeviceToken> UserDeviceToken { get; set; }

    public virtual DbSet<MuscleGroups> MuscleGroups { get; set; }

    public virtual DbSet<Routines> Routines { get; set; }

    public virtual DbSet<Exercises> Exercises { get; set; }

    public virtual DbSet<RoutineExercises> RoutineExercises { get; set; }

    public virtual DbSet<AthleteRoutines> AthleteRoutines { get; set; }

    public virtual DbSet<RoutineExerciseSets> RoutineExerciseSets { get; set; }

    public virtual DbSet<HistoricalSets> HistoricalSets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
