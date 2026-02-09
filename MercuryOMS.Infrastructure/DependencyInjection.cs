using MediatR;
using MercuryOMS.Application.Interfaces;
using MercuryOMS.Infrastructure.Data;
using MercuryOMS.Infrastructure.Data.Interceptors;
using MercuryOMS.Infrastructure.Implementations;
using MercuryOMS.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MercuryOMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddPostgresDbContext(configuration);
            services.AddRepository();
            return services;
        }

        private static IServiceCollection AddPostgresDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("MercuryOMSDatabase"));

                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            services.AddScoped<AuditSaveChangesInterceptor>();
            return services;
        }

        private static IServiceCollection AddRepository(
            this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
