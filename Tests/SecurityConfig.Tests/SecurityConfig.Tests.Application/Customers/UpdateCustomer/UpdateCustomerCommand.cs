using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SecurityConfig.Tests.Application.Common.Interfaces;
using SecurityConfig.Tests.Application.Common.Security;
using SecurityConfig.Tests.Application.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Customers.UpdateCustomer
{
    [Authorize(Policy = Permissions.PolicyCustomer)]
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(string name, string surname, Guid id)
        {
            Name = name;
            Surname = surname;
            Id = id;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Id { get; set; }
    }
}