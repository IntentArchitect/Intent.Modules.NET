using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Invoice" })]
    internal class InvoiceDocument : IInvoiceDocument, IRedisOmDocument<Invoice, InvoiceDocument>
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Number { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string ClientIdentifier { get; set; } = default!;
        public List<LineItemDocument> LineItems { get; set; } = default!;
        IReadOnlyList<ILineItemDocument> IInvoiceDocument.LineItems => LineItems;
        public InvoiceLogoDocument InvoiceLogo { get; set; } = default!;
        IInvoiceLogoDocument IInvoiceDocument.InvoiceLogo => InvoiceLogo;

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Invoice>();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Date = Date;
            entity.Number = Number ?? throw new Exception($"{nameof(entity.Number)} is null");
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.CreatedDate = CreatedDate;
            entity.UpdatedBy = UpdatedBy;
            entity.UpdatedDate = UpdatedDate;
            entity.ClientIdentifier = ClientIdentifier ?? throw new Exception($"{nameof(entity.ClientIdentifier)} is null");
            entity.LineItems = LineItems.Select(x => x.ToEntity()).ToList();
            entity.InvoiceLogo = InvoiceLogo.ToEntity() ?? throw new Exception($"{nameof(entity.InvoiceLogo)} is null");

            return entity;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity)
        {
            Id = entity.Id;
            Date = entity.Date;
            Number = entity.Number;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            UpdatedBy = entity.UpdatedBy;
            UpdatedDate = entity.UpdatedDate;
            ClientIdentifier = entity.ClientIdentifier;
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