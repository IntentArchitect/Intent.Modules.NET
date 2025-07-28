using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("invoices")]
    internal class InvoiceDocument : IDynamoDBDocument<IInvoice, Invoice, InvoiceDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string ClientIdentifier { get; set; } = default!;
        public DateTime Date { get; set; }
        public string Number { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        [DynamoDBVersion]
        public int? Version { get; set; }
        public List<LineItemDocument> LineItems { get; set; } = default!;
        public InvoiceLogoDocument InvoiceLogo { get; set; } = default!;

        public object GetKey() => Id;

        public int? GetVersion() => Version;

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

        public InvoiceDocument PopulateFromEntity(IInvoice entity, Func<object, int?> getVersion)
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
            Version ??= getVersion(GetKey());

            return this;
        }

        public static InvoiceDocument? FromEntity(IInvoice? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}