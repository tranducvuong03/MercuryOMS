using Microsoft.Extensions.DependencyInjection;

namespace MercuryOMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddApplicationMediatR();
            services.AddApplicationAutoMapper();
            return services;
        }

        private static IServiceCollection AddApplicationMediatR(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly
                ));
            return services;
        }

        private static IServiceCollection AddApplicationAutoMapper(
            this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
