using Application.Interfaces;
using Application.Services;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMotoService, MotoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEntregadorService, EntregadorService>();
            services.AddScoped<ICnhStorageService, LocalCnhStorageService>();
            services.AddScoped<ILocacaoService, LocacaoService>();
            services.AddScoped<IMessagingService, RabbitMqService>();

            return services;
        }
    }
}
