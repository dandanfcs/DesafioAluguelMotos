using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Messaging;
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
            services.AddScoped<ICnhStorageService, LocalCnhStorageService>();

            services.AddScoped<IMessagingService, RabbitMqService>();
            services.AddScoped<IMotoNotificacaoRepository, MotoNotificacaoRepository>();
         

            return services;
        }
    }
}
