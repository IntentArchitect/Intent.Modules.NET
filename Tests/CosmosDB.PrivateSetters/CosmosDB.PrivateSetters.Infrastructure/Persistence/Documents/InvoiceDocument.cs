using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : IInvoiceDocument, ICosmosDBDocument<Invoice, InvoiceDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
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

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(ClientIdentifier), ClientIdentifier ?? throw new Exception($"{nameof(entity.ClientIdentifier)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Date), Date);
            ReflectionHelper.ForceSetProperty(entity, nameof(Number), Number ?? throw new Exception($"{nameof(entity.Number)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(CreatedBy), CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(CreatedDate), CreatedDate);
            ReflectionHelper.ForceSetProperty(entity, nameof(UpdatedBy), UpdatedBy);
            ReflectionHelper.ForceSetProperty(entity, nameof(UpdatedDate), UpdatedDate);
            ReflectionHelper.ForceSetProperty(entity, nameof(LineItems), LineItems.Select(x => x.ToEntity()).ToList());
            ReflectionHelper.ForceSetProperty(entity, nameof(InvoiceLogo), InvoiceLogo.ToEntity() ?? throw new Exception($"{nameof(entity.InvoiceLogo)} is null"));

            return entity;
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