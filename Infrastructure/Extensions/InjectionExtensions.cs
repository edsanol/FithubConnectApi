using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
        {
            var assembly = typeof(DbFithubContext).Assembly.FullName;

            services.AddDbContext<DbFithubContext>(
                options => options.UseNpgsql(
                    connectionString,
                    a => a.MigrationsAssembly(assembly)),
                    ServiceLifetime.Transient);

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
