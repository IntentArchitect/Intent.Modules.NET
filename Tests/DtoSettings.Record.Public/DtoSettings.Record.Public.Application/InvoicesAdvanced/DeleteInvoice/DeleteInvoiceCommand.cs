using System;
using DtoSettings.Record.Public.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.InvoicesAdvanced.DeleteInvoice
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