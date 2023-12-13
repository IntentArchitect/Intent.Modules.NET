using CosmosDBMultiTenancy.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDBMultiTenancy.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateInvoiceCommand(string number)
        {
            Number = number;
        }

        public string Number { get; set; }
    }
}