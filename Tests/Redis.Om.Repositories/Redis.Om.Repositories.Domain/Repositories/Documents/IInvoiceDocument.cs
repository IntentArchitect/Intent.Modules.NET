using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocumentInterface", Version = "1.0")]

namespace Redis.Om.Repositories.Domain.Repositories.Documents
{
    public interface IInvoiceDocument
    {
        string Id { get; }
        DateTime Date { get; }
        string Number { get; }
        string CreatedBy { get; }
        DateTimeOffset CreatedDate { get; }
        string? UpdatedBy { get; }
        DateTimeOffset? UpdatedDate { get; }
        string ClientIdentifier { get; }
        IReadOnlyList<ILineItemDocument> LineItems { get; }
        IInvoiceLogoDocument InvoiceLogo { get; }
    }
}