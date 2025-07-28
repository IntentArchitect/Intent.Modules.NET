using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class InvoiceLogoDocument
    {
        public string Url { get; set; } = default!;

        public InvoiceLogo ToEntity(InvoiceLogo? entity = default)
        {
            entity ??= new InvoiceLogo();

            entity.Url = Url ?? throw new Exception($"{nameof(entity.Url)} is null");

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