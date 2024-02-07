using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class InvoiceLogoDocument : IInvoiceLogoDocument
    {
        public string Url { get; set; } = default!;

        public InvoiceLogo ToEntity(InvoiceLogo? entity = default)
        {
            entity ??= new InvoiceLogo();

            entity.Url = Url ?? throw new Exception($"{nameof(entity.Url)} is null");

            return entity;
        }

        public InvoiceLogoDocument PopulateFromEntity(InvoiceLogo entity, string? etag = null)
        {
            Url = entity.Url;

            this.etag = etag;

            return this;
        }

        public static InvoiceLogoDocument? FromEntity(InvoiceLogo? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceLogoDocument().PopulateFromEntity(entity, etag);
        }
    }
}