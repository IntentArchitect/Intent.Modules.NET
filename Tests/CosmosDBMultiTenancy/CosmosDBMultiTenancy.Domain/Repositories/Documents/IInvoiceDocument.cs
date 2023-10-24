using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDBMultiTenancy.Domain.Repositories.Documents
{
    public interface IInvoiceDocument
    {
        string Id { get; }
        string Number { get; }
        string TenantId { get; }
        string CreatedBy { get; }
        DateTimeOffset CreatedDate { get; }
        string? UpdatedBy { get; }
        DateTimeOffset? UpdatedDate { get; }
    }
}