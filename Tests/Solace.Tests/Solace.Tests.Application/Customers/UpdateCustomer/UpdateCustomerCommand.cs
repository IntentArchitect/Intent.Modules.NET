using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Solace.Tests.Application.Customers.UpdateCustomer
{
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