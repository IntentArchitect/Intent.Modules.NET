using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoFramework;
using MongoFramework.Infrastructure.Mapping;
using Subscribe.GooglePubSub.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.ApplicationMongoDbContext", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Infrastructure.Persistence
{
    public class ApplicationMongoDbContext : MongoDbContext, IMongoDbUnitOfWork
    {
        public ApplicationMongoDbContext(IMongoDbConnection connection) : base(connection)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            return default;
        }

        protected override void Dispose(bool disposing)
        {
            // Don't call the base's dispose as it disposes the connection which is not recommended as per https://www.mongodb.com/docs/manual/administration/connection-pool-overview/
        }

        protected override void OnConfigureMapping(MappingBuilder mappingBuilder)
        {
            base.OnConfigureMapping(mappingBuilder);
        }
    }
}