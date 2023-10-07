using System;
using System.Collections.Generic;
using GraphQL.CQRS.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest<InvoiceDto>, ICommand
    {
        public UpdateInvoiceCommand(Guid id, int no, DateTime created, Guid customerId)
        {
            Id = id;
            No = no;
            Created = created;
            CustomerId = customerId;
        }
        public Guid Id { get; private set; }

        public int No { get; set; }

        public DateTime Created { get; set; }

        public Guid CustomerId { get; set; }

        public void SetId(Guid id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }

    }
}