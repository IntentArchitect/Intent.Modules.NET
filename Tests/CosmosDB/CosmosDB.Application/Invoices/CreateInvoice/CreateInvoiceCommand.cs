using System;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateInvoiceCommand(string clientId, DateTime date, string number)
        {
            ClientId = clientId;
            Date = date;
            Number = number;
        }

        public string ClientId { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }
}