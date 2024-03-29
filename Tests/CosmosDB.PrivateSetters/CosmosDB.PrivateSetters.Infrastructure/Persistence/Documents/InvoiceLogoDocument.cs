using System;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class InvoiceLogoDocument : IInvoiceLogoDocument
    {
        public string Url { get; set; } = default!;

        public InvoiceLogo ToEntity(InvoiceLogo? entity = default)
        {
            entity ??= new InvoiceLogo();

            ReflectionHelper.ForceSetProperty(entity, nameof(Url), Url ?? throw new Exception($"{nameof(entity.Url)} is null"));

            return entity;
        }

        public InvoiceLogoDocument PopulateFromEntity(InvoiceLogo entity)
        {
            Url = entity.Url;

            return this;
        }

        public static InvoiceLogoDocument? FromEntity(InvoiceLogo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceLogoDocument().PopulateFromEntity(entity);
        }
    }
}