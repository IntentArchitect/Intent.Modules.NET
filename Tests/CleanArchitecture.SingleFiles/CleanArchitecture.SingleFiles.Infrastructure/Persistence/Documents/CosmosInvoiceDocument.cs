using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents
{
    internal class CosmosInvoiceDocument : ICosmosInvoiceDocument, ICosmosDBDocument<CosmosInvoice, CosmosInvoiceDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<CosmosLineDocument> CosmosLines { get; set; } = default!;
        IReadOnlyList<ICosmosLineDocument> ICosmosInvoiceDocument.CosmosLines => CosmosLines;

        public CosmosInvoice ToEntity(CosmosInvoice? entity = default)
        {
            entity ??= new CosmosInvoice();

            entity.Id = Id;
            entity.Description = Description;
            entity.CosmosLines = CosmosLines.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public CosmosInvoiceDocument PopulateFromEntity(CosmosInvoice entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            CosmosLines = entity.CosmosLines.Select(x => CosmosLineDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static CosmosInvoiceDocument? FromEntity(CosmosInvoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CosmosInvoiceDocument().PopulateFromEntity(entity);
        }
    }
}