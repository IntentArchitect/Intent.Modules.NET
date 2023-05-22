using System;
using System.Collections.Generic;
using GraphQL.CQRS.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateInvoiceCommand(int no, DateTime created, Guid customerId)
        {
            No = no;
            Created = created;
            CustomerId = customerId;
        }
        public int No { get; set; }

        public DateTime Created { get; set; }

        public Guid CustomerId { get; set; }

    }
}