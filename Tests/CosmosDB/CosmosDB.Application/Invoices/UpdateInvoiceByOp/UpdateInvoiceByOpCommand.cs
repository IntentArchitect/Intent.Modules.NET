using System;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.UpdateInvoiceByOp
{
    public class UpdateInvoiceByOpCommand : IRequest, ICommand
    {
        public UpdateInvoiceByOpCommand(string id, DateTime date, string number, string clientIdentifier)
        {
            Id = id;
            Date = date;
            Number = number;
            ClientIdentifier = clientIdentifier;
        }

        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string ClientIdentifier { get; set; }
    }
}