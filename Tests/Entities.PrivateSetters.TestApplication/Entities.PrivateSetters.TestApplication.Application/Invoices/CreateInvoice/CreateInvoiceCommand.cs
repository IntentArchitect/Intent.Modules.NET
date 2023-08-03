using System;
using System.Collections.Generic;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateInvoiceCommand(DateTime date, List<CreateInvoiceLineDataContractDto> lines)
        {
            Date = date;
            Lines = lines;
        }

        public DateTime Date { get; set; }
        public List<CreateInvoiceLineDataContractDto> Lines { get; set; }
    }
}