using System;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : Invoice, ICosmosDBDocument<InvoiceDocument, Invoice>
    {
        private string? _type;
        [JsonProperty("id")]
        public new string Id
        {
            get => base.Id ??= Guid.NewGuid().ToString();
            set => base.Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity)
        {
            Id = entity.Id;
            ClientIdentifier = entity.ClientIdentifier;
            Date = entity.Date;
            Number = entity.Number;
            LineItems = entity.LineItems;

            return this;
        }
    }
}