using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SecurityConfig.Tests.Application.Common.Interfaces;
using SecurityConfig.Tests.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Customers.CreateCustomer
{
    [Authorize(Policy = "Customer")]
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
    }
}