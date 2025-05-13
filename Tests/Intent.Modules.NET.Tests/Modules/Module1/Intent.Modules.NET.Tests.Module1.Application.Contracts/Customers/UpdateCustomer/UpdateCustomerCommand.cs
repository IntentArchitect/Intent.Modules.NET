using Intent.Modules.NET.Tests.Application.Core.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}