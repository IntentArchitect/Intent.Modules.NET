using System;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Kafka.Producer.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(Guid id, string note)
        {
            Id = id;
            Note = note;
        }

        public Guid Id { get; set; }
        public string Note { get; set; }
    }
}