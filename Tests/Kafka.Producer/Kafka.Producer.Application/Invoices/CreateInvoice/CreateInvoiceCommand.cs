using System;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Kafka.Producer.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateInvoiceCommand(string note)
        {
            Note = note;
        }

        public string Note { get; set; }
    }
}