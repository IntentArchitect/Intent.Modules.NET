using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using CosmosDB.EntityInterfaces.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IInvoice : IHasDomainEvent, IAuditable
    {
        string Id { get; set; }

        string ClientIdentifier { get; set; }

        DateTime Date { get; set; }

        string Number { get; set; }

        string CreatedBy { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        string? UpdatedBy { get; set; }

        DateTimeOffset? UpdatedDate { get; set; }

        ICollection<ILineItem> LineItems { get; set; }

        IInvoiceLogo InvoiceLogo { get; set; }

        void Update(DateTime date, string number, string clientIdentifier);
    }
}