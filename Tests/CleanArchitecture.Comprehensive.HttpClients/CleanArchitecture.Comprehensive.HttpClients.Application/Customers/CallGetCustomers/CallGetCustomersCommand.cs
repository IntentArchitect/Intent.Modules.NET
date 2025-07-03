using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallGetCustomers
{
    public class CallGetCustomersCommand : IRequest, ICommand
    {
        public CallGetCustomersCommand()
        {
        }
    }
}