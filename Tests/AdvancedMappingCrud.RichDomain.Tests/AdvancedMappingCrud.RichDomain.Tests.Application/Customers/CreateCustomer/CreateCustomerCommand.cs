using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(CreateCustomerUserDto user, string login)
        {
            User = user;
            Login = login;
        }

        public CreateCustomerUserDto User { get; set; }
        public string Login { get; set; }
    }
}