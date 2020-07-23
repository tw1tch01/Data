using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IContextRepository<>), typeof(ContextRepository<>));
            services.AddLogging();

            return services;
        }
    }
}