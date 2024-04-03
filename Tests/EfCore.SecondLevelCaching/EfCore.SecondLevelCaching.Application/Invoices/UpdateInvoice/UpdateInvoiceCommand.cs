using System;
using EfCore.SecondLevelCaching.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EfCore.SecondLevelCaching.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(Guid id, string notes)
        {
            Id = id;
            Notes = notes;
        }

        public Guid Id { get; set; }
        public string Notes { get; set; }
    }
}