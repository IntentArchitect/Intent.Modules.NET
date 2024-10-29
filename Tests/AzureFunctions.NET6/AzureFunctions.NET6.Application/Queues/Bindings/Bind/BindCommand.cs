using AzureFunctions.NET6.Application.Common.Interfaces;
using AzureFunctions.NET6.Application.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Queues.Bindings.Bind
{
    public class BindCommand : IRequest<CustomerDto>, ICommand
    {
        public BindCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}