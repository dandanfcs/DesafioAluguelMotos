using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Infrastructure.Data;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove DbContext real
            var appDbContext = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (appDbContext != null)
                services.Remove(appDbContext);

            var applicationUserDbContext = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationUserDbContext>));
            if (applicationUserDbContext != null)
                services.Remove(applicationUserDbContext);

            // Adiciona InMemoryDb
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });


            // Substitui autenticação por uma fake
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, EntregadorTestAuthHandler>(
                    "Test", options => { });
        });

        builder.UseEnvironment("Development");
    }
}
