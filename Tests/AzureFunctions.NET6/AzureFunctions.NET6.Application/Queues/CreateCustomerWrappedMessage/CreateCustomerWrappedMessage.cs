using AzureFunctions.NET6.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Queues.CreateCustomerWrappedMessage
{
    public class CreateCustomerWrappedMessage : IRequest, ICommand
    {
        public CreateCustomerWrappedMessage(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}