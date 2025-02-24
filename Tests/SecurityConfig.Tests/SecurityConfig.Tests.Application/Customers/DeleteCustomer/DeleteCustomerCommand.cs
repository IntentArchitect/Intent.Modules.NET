using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SecurityConfig.Tests.Application.Common.Interfaces;
using SecurityConfig.Tests.Application.Common.Security;
using SecurityConfig.Tests.Application.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SecurityConfig.Tests.Application.Customers.DeleteCustomer
{
    [Authorize(Policy = $"{Permissions.PolicyCustomer},{Permissions.PolicyAdmin}")]
    public class DeleteCustomerCommand : IRequest, ICommand
    {
        public DeleteCustomerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}