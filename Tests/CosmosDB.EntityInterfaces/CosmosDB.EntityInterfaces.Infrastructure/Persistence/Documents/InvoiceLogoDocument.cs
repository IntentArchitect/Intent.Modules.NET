using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class InvoiceLogoDocument : IInvoiceLogoDocument
    {
        public string Url { get; set; } = default!;

        public InvoiceLogo ToEntity(InvoiceLogo? entity = default)
        {
            entity ??= new InvoiceLogo();

            entity.Url = Url;

            return entity;
        }

        public InvoiceLogoDocument PopulateFromEntity(IInvoiceLogo entity)
        {
            Url = entity.Url;

            return this;
        }

        public static InvoiceLogoDocument? FromEntity(IInvoiceLogo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceLogoDocument().PopulateFromEntity(entity);
        }
    }
}