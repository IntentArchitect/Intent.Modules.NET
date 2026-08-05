using System.Reflection;
using IntegrationTesting.SQLLite.Tests.Domain.Common.Interfaces;
using IntegrationTesting.SQLLite.Tests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.EFContainerFixture", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests
{
    public class EFContainerFixture
    {
        private readonly SqliteConnection _dbConnection;

        public EFContainerFixture()
        {
            _dbConnection = new SqliteConnection("Filename=:memory:");
        }

        public void ConfigureTestServices(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlite(
                    _dbConnection,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            //Schema Creation
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }

        public void OnHostCreation(IServiceProvider services)
        {
        }

        public async Task InitializeAsync()
        {
            await _dbConnection.OpenAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbConnection.DisposeAsync();
        }
    }
}