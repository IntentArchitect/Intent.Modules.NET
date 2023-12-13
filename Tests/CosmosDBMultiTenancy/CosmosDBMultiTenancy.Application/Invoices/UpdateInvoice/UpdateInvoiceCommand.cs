using CosmosDBMultiTenancy.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDBMultiTenancy.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(string id, string number)
        {
            Id = id;
            Number = number;
        }

        public string Id { get; set; }
        public string Number { get; set; }
    }
}