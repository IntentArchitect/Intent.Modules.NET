using System;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(string id, string clientId, DateTime date, string number)
        {
            Id = id;
            ClientId = clientId;
            Date = date;
            Number = number;
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }
}