using System;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Kafka.Consumer.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateInvoiceCommand(Guid id, string note)
        {
            Id = id;
            Note = note;
        }

        public Guid Id { get; set; }

        public string Note { get; set; }
    }
}