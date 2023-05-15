using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Domain.Common.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoFramework;
using MongoFramework.Infrastructure.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.ApplicationMongoDbContext", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Persistence
{
    public class ApplicationMongoDbContext : MongoDbContext, IMongoDbUnitOfWork
    {
        public ApplicationMongoDbContext(IMongoDbConnection connection) : base(connection)
        {
        }

        public MongoDbSet<Invoice> Invoices { get; set; }
        public MongoDbSet<Tag> Tags { get; set; }

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
            mappingBuilder.Entity<Invoice>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));

            mappingBuilder.Entity<Tag>()
                .HasKey(e => e.Id, d => d.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator));
            base.OnConfigureMapping(mappingBuilder);
        }
    }
}