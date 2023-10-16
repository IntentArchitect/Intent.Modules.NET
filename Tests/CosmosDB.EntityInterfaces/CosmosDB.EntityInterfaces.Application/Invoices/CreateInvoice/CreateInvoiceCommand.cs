using System;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateInvoiceCommand(string clientIdentifier, DateTime date, string number)
        {
            ClientIdentifier = clientIdentifier;
            Date = date;
            Number = number;
        }

        public string ClientIdentifier { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }
}