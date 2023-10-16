using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using MultipleDocumentStores.Domain.Common;
using MultipleDocumentStores.Domain.Entities;
using MultipleDocumentStores.Domain.Repositories.Documents;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Persistence.Documents
{
    internal class CustomerCosmosDocument : ICustomerCosmosDocument, ICosmosDBDocument<CustomerCosmos, CustomerCosmosDocument>
    {
        private string? _type;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public CustomerCosmos ToEntity(CustomerCosmos? entity = default)
        {
            entity ??= new CustomerCosmos();

            entity.Id = Id;
            entity.Name = Name;

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }

        public CustomerCosmosDocument PopulateFromEntity(CustomerCosmos entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CustomerCosmosDocument? FromEntity(CustomerCosmos? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerCosmosDocument().PopulateFromEntity(entity);
        }
    }
}