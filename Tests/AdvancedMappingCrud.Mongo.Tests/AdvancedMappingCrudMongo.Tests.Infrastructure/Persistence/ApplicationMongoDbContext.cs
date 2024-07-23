using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Common;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MongoFramework;
using MongoFramework.Infrastructure.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.ApplicationMongoDbContext", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence
{
    public class ApplicationMongoDbContext : MongoDbContext, IMongoDbUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationMongoDbContext(IMongoDbConnection connection, IDomainEventService domainEventService) : base(connection)
        {
            _domainEventService = domainEventService;
        }

        public MongoDbSet<Customer> Customers { get; set; }
        public MongoDbSet<ExternalDoc> ExternalDocs { get; set; }
        public MongoDbSet<Order> Orders { get; set; }
        public MongoDbSet<Product> Products { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents(cancellationToken);
            await base.SaveChangesAsync(cancellationToken);
            return default;
        }

        protected override void Dispose(bool disposing)
        {
            // Don't call the base's dispose as it disposes the connection which is not recommended as per https://www.mongodb.com/docs/manual/administration/connection-pool-overview/
        }

        private async Task DispatchEvents(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries()
                    .Select(x => x.Entity)
                    .OfType<IHasDomainEvent>()
                    .SelectMany(x => x.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }

        protected override void OnConfigureMapping(MappingBuilder mappingBuilder)
        {

            mappingBuilder.Entity<Customer>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .Ignore(entity => entity.DomainEvents);

            mappingBuilder.Entity<ExternalDoc>()
                .HasKey(entity => entity.Id)
                .Ignore(entity => entity.DomainEvents);

            mappingBuilder.Entity<Order>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .Ignore(entity => entity.DomainEvents);

            mappingBuilder.Entity<Product>()
                .HasKey(entity => entity.Id, build => build.HasKeyGenerator(EntityKeyGenerators.StringKeyGenerator))
                .Ignore(entity => entity.DomainEvents);
            base.OnConfigureMapping(mappingBuilder);
        }
    }
}