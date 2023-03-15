using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Infrastructure;
using MongoDb.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.ApplicationMongoDbContext", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence
{
    public class ApplicationMongoDbContext : MongoDbContext, IMongoDbUnitOfWork
    {
        static ApplicationMongoDbContext()
        {
            ApplyConfigurationsFromAssembly(typeof(ApplicationMongoDbContext).Assembly);
        }

        public ApplicationMongoDbContext(string connectionString, string databaseName, MongoDatabaseSettings databaseSettings = null) : base(connectionString, databaseName, databaseSettings)
        {
            AcceptAllChangesOnSave = true;
            AddCommand(() => null);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return (await base.SaveChangesAsync(cancellationToken)).Results.Count;
        }
    }
}