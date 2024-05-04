using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(string id, int number, string clientId)
        {
            Id = id;
            Number = number;
            ClientId = clientId;
        }

        public string Id { get; set; }
        public int Number { get; set; }
        public string ClientId { get; set; }
    }
}