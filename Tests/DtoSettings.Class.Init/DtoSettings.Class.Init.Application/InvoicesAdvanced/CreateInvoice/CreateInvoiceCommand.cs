using System;
using System.Collections.Generic;
using DtoSettings.Class.Init.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.InvoicesAdvanced.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateInvoiceCommand(string number, List<CreateInvoiceCommandInvoiceLinesDto> invoiceLines)
        {
            Number = number;
            InvoiceLines = invoiceLines;
        }

        public string Number { get; set; }
        public List<CreateInvoiceCommandInvoiceLinesDto> InvoiceLines { get; set; }
    }
}