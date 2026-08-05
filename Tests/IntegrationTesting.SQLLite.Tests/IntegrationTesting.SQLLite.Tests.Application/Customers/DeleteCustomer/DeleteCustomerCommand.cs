using IntegrationTesting.SQLLite.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest, ICommand
    {
        public DeleteCustomerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}