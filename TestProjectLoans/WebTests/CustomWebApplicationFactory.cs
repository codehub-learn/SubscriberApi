 
namespace TestProjectLoans.WebTests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SubscriberApi.Data;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // REMOVE EVERYTHING EF RELATED
            var descriptors = services
                .Where(d =>
                    d.ServiceType.FullName != null &&
                    (
                        d.ServiceType.FullName.Contains("DbContext")
                        ||
                        d.ServiceType.FullName.Contains("EntityFramework")
                    ))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // ADD ONLY InMemory
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    Guid.NewGuid().ToString());
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
        });
    }
}