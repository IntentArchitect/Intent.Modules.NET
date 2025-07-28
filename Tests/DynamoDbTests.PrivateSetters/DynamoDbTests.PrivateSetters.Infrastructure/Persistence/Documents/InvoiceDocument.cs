using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("invoices")]
    internal class InvoiceDocument : IDynamoDBDocument<Invoice, InvoiceDocument>
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

        public InvoiceDocument PopulateFromEntity(Invoice entity, Func<object, int?> getVersion)
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

        public static InvoiceDocument? FromEntity(Invoice? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}