using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallGetCustomerById
{
    public class CallGetCustomerByIdCommand : IRequest, ICommand
    {
        public CallGetCustomerByIdCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}