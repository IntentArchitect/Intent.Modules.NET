using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class InvoiceLogoDocument
    {
        public string Url { get; set; } = default!;

        public InvoiceLogo ToEntity(InvoiceLogo? entity = default)
        {
            entity ??= new InvoiceLogo();

            entity.Url = Url;

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