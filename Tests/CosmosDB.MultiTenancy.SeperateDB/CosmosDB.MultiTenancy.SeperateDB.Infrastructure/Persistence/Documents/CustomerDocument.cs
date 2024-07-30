using System;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Entities;
using CosmosDB.MultiTenancy.SeperateDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence.Documents
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

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;

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