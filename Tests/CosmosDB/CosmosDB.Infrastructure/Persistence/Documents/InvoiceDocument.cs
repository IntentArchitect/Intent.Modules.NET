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
        [JsonProperty("_etag")]
        protected string? _etag;
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

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ClientIdentifier = ClientIdentifier ?? throw new Exception($"{nameof(entity.ClientIdentifier)} is null");
            entity.Date = Date;
            entity.Number = Number ?? throw new Exception($"{nameof(entity.Number)} is null");
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.CreatedDate = CreatedDate;
            entity.UpdatedBy = UpdatedBy;
            entity.UpdatedDate = UpdatedDate;
            entity.LineItems = LineItems.Select(x => x.ToEntity()).ToList();
            entity.InvoiceLogo = InvoiceLogo.ToEntity() ?? throw new Exception($"{nameof(entity.InvoiceLogo)} is null");

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;

        public InvoiceDocument PopulateFromEntity(Invoice entity, Func<string, string?> getEtag)
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

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static InvoiceDocument? FromEntity(Invoice? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}