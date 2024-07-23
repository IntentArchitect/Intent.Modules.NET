using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class CustomerDocument : ICustomerDocument, ICosmosDBDocument<Customer, CustomerDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public bool IsActive { get; set; }
        public AddressDocument ShippingAddress { get; set; } = default!;
        IAddressDocument ICustomerDocument.ShippingAddress => ShippingAddress;

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Surname = Surname ?? throw new Exception($"{nameof(entity.Surname)} is null");
            entity.IsActive = IsActive;
            entity.ShippingAddress = ShippingAddress.ToEntity() ?? throw new Exception($"{nameof(entity.ShippingAddress)} is null");

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            Surname = entity.Surname;
            IsActive = entity.IsActive;
            ShippingAddress = AddressDocument.FromEntity(entity.ShippingAddress)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static CustomerDocument? FromEntity(Customer? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}