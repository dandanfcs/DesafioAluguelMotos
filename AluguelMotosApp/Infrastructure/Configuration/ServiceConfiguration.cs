using Application.Interfaces;
using Application.Services;
using Domain;
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

            return services;
        }
    }
}
