using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Infrastructure;
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

        async Task<int> IMongoDbUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return default;
        }

        async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            return default;
        }

        protected override void Dispose(bool disposing)
        {
            //we don't want to dispose the connection which the base class does
        }

        protected override void OnConfigureMapping(MappingBuilder mappingBuilder)
        {
            base.OnConfigureMapping(mappingBuilder);
        }
    }
}