using System;
using EfCore.SecondLevelCaching.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Application.Invoices.DeleteInvoice
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