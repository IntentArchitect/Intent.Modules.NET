using System;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.ChangeEmailCustomer
{
    public class ChangeEmailCustomerCommand : IRequest, ICommand
    {
        public ChangeEmailCustomerCommand(Guid id, string email)
        {
            Id = id;
            Email = email;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}