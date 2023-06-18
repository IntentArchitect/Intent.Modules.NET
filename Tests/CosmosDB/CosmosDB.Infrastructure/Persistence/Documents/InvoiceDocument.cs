using System;
using System.Collections.Generic;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : IItem
    {
        private string? _type;

        public InvoiceDocument()
        {
            Id = null!;
            ClientIdentifier = null!;
            Number = null!;
            LineItems = null!;
        }

        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => Id;
        [JsonProperty("id")]
        public string Id { get; set; }
        public string ClientIdentifier { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public ICollection<LineItem> LineItems { get; set; }
    }
}