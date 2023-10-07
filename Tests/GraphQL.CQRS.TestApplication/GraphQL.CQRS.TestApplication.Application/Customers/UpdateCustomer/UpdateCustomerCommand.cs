using System;
using System.Collections.Generic;
using GraphQL.CQRS.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<CustomerDto>, ICommand
    {
        public UpdateCustomerCommand(Guid id, string name, string surname, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
        }
        public Guid Id { get; private set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public void SetId(Guid id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }

    }
}