using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SecurityConfig.Tests.Application.Common.Interfaces;
using SecurityConfig.Tests.Application.Common.Security;
using SecurityConfig.Tests.Application.Security;
using SecurityConfig.Tests.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Customers.CreateCustomer
{
    [Authorize(Policy = Permissions.PolicyCustomer)]
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string surname, CustomerType customerType)
        {
            Name = name;
            Surname = surname;
            CustomerType = customerType;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public CustomerType CustomerType { get; set; }
    }
}