using System;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Kafka.Producer.Application.Invoices.DeleteInvoice
{
    public class DeleteInvoiceCommand : IRequest, ICommand
    {
        public DeleteInvoiceCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}