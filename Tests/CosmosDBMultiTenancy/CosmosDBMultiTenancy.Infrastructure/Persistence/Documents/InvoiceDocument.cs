using System;
using System.Collections.Generic;
using CosmosDBMultiTenancy.Domain.Common;
using CosmosDBMultiTenancy.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : Invoice, ICosmosDBDocument<InvoiceDocument, Invoice>
    {
        private string? _type;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Id;
            set => Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string IItem.PartitionKey => TenantId;
        [JsonIgnore]
        public override List<DomainEvent> DomainEvents
        {
            get => base.DomainEvents;
            set => base.DomainEvents = value;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity)
        {
            Id = entity.Id;
            Number = entity.Number;
            TenantId = entity.TenantId;

            return this;
        }
    }
}