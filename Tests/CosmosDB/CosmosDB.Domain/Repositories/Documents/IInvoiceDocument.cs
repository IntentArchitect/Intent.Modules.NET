using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Documents
{
    public interface IInvoiceDocument
    {
        string Id { get; }
        string ClientIdentifier { get; }
        DateTime Date { get; }
        string Number { get; }
        string CreatedBy { get; }
        DateTimeOffset CreatedDate { get; }
        string? UpdatedBy { get; }
        DateTimeOffset? UpdatedDate { get; }
        IReadOnlyList<ILineItemDocument> LineItems { get; }
        IInvoiceLogoDocument InvoiceLogo { get; }
    }
}