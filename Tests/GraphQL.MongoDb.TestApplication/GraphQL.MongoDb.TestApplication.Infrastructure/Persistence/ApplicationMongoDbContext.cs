using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Common;
using GraphQL.MongoDb.TestApplication.Domain.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoFramework;
using MongoFramework.Infrastructure.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.ApplicationMongoDbContext", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Persistence
{
    public class ApplicationMongoDbContext : MongoDbContext, IMongoDbUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationMongoDbContext(IMongoDbConnection connection, IDomainEventService domainEventService) : base(connection)
        {
            _domainEventService = domainEventService;
        }

        public MongoDbSet<Privilege> Privileges { get; set; }
        public MongoDbSet<User> Users { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents();
            await base.SaveChangesAsync(cancellationToken);
            return default;
        }

        protected override void Dispose(bool disposing)
        {
            // Don't call the base's dispose as it disposes the connection which is not recommended as per https://www.mongodb.com/docs/manual/administration/connection-pool-overview/
        }

        private async Task DispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries()
                    .Select(x => x.Entity)
                    .OfType<IHasDomainEvent>()
                    .SelectMany(x => x.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }

        protected override void OnConfigureMapping(MappingBuilder mappingBuilder)
        {
            mappingBuilder.Entity<Privilege>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .Ignore(entity => entity.DomainEvents);

            mappingBuilder.Entity<User>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .Ignore(entity => entity.DomainEvents);
            base.OnConfigureMapping(mappingBuilder);
        }
    }
}