using System;
using System.IO;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DesignTimeDbContextFactory.DesignTimeDbContextFactory", Version = "1.0")]

namespace AzureFunction.QueueStorage.Infrastructure.Persistence
{
    /// <summary>
    /// In the event that one cannot run EF Core CLI commands due to Startup app constraints,
    /// having this class present will bypass your startup app and rather look at an appsettings.json file
    /// locally for connection-string info to construct an <see cref="ApplicationDbContext"/>. 
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <inheritdoc />
        /// <param name="args">
        /// This is optional but will only accept 1 parameter which is the name of the connection string to lookup
        /// in a local appsettings.json file. By default this will use "DefaultConnection".
        /// </param>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            throw new NotSupportedException("Cannot create migration scripts for In-Memory database provider");
        }
    }
}