using System;
using EntityFrameworkCore.Oracle.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(Guid id, string number)
        {
            Id = id;
            Number = number;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }
    }
}