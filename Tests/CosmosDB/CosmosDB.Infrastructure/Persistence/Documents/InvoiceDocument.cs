using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Interfaces;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : IInvoiceDocument, ICosmosDBDocument<Invoice, InvoiceDocument>
    {
        private string? _type;
        public string Id { get; set; } = default!;
        public string ClientIdentifier { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Number { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public List<LineItemDocument> LineItems { get; set; } = default!;
        IReadOnlyList<ILineItemDocument> IInvoiceDocument.LineItems => LineItems;
        public InvoiceLogoDocument InvoiceLogo { get; set; } = default!;
        IInvoiceLogoDocument IInvoiceDocument.InvoiceLogo => InvoiceLogo;

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Invoice>();

            entity.Id = Id;
            entity.ClientIdentifier = ClientIdentifier;
            entity.Date = Date;
            entity.Number = Number;
            entity.CreatedBy = CreatedBy;
            entity.CreatedDate = CreatedDate;
            entity.UpdatedBy = UpdatedBy;
            entity.UpdatedDate = UpdatedDate;
            entity.LineItems = LineItems.Select(x => x.ToEntity()).ToList();
            entity.InvoiceLogo = InvoiceLogo.ToEntity()!;

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity)
        {
            Id = entity.Id;
            ClientIdentifier = entity.ClientIdentifier;
            Date = entity.Date;
            Number = entity.Number;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            UpdatedBy = entity.UpdatedBy;
            UpdatedDate = entity.UpdatedDate;
            LineItems = entity.LineItems.Select(x => LineItemDocument.FromEntity(x)!).ToList();
            InvoiceLogo = InvoiceLogoDocument.FromEntity(entity.InvoiceLogo)!;

            return this;
        }

        public static InvoiceDocument? FromEntity(Invoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity);
        }
    }
}