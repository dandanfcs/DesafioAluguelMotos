using Application.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));

            services.AddDbContexts(configuration);
            services.AddIdentityServices();
            services.AddJwtAuthentication(configuration);
            services.AddRepositories();
            services.AddApplicationServices();

            return services;
        }
    }
}
