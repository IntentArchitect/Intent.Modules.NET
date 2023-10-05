using CleanArchitecture.OnlyModeledDomainEvents.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Infrastructure.Persistence.Documents
{
    internal class CustomerDocument : ICosmosDBDocument<Customer, CustomerDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Id;

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity)
        {
            Id = entity.Id;

            return this;
        }

        public static CustomerDocument? FromEntity(Customer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerDocument().PopulateFromEntity(entity);
        }
    }
}