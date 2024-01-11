using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class CustomerDocument : ICustomerDocument, ICosmosDBDocument<Customer, CustomerDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public AddressDocument DeliveryAddress { get; set; } = default!;
        IAddressDocument ICustomerDocument.DeliveryAddress => DeliveryAddress;
        public AddressDocument? BillingAddress { get; set; }
        IAddressDocument ICustomerDocument.BillingAddress => BillingAddress;

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.DeliveryAddress = DeliveryAddress.ToEntity() ?? throw new Exception($"{nameof(entity.DeliveryAddress)} is null");
            entity.BillingAddress = BillingAddress?.ToEntity();

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            DeliveryAddress = AddressDocument.FromEntity(entity.DeliveryAddress)!;
            BillingAddress = AddressDocument.FromEntity(entity.BillingAddress);

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