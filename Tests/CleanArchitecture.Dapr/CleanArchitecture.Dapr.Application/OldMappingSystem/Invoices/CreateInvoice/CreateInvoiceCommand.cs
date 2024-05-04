using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<string>, ICommand
    {
        public CreateInvoiceCommand(int number, string clientId)
        {
            Number = number;
            ClientId = clientId;
        }

        public int Number { get; set; }
        public string ClientId { get; set; }
    }
}