using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MetaMeta.Configs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<ConfigurationProvider>();
            return services;
        }
    }
} 