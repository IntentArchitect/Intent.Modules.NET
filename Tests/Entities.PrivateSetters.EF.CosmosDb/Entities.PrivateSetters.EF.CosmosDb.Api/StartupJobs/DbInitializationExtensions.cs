using System;
using System.Threading.Tasks;
using Entities.PrivateSetters.EF.CosmosDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbInitializationExtensions", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Api.StartupJobs
{
    public static class DbInitializationExtensions
    {
        /// <summary>
        /// Performs a check to see whether the database exist and if not will create it
        /// based on the EntityFrameworkCore DbContext configuration.
        /// </summary>
        public static async Task EnsureDbCreationAsync(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                throw new InvalidOperationException("DbContext not configured in Services Collection in order to ensure that the database is created.");
            }

            await dbContext.EnsureDbCreatedAsync();
        }
    }
}