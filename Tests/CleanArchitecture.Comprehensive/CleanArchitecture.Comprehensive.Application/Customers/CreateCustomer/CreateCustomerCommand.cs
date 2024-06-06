using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string surname, string email, CreateCustomerAddressDto address)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Address = address;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public CreateCustomerAddressDto Address { get; set; }
    }
}