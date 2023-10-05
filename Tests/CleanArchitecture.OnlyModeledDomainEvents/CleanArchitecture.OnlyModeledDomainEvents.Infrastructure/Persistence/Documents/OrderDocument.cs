using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence.Documents
{
    internal class OrderDocument : ICosmosDBDocument<Order, OrderDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            entity.Id = Id;

            return entity;
        }

        public OrderDocument PopulateFromEntity(Order entity)
        {
            Id = entity.Id;

            return this;
        }

        public static OrderDocument? FromEntity(Order? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderDocument().PopulateFromEntity(entity);
        }
    }
}