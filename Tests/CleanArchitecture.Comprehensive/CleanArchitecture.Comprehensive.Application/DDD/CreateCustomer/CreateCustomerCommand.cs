using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string surname, CreateCustomerAddressDto address, string email)
        {
            Name = name;
            Surname = surname;
            Address = address;
            Email = email;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public CreateCustomerAddressDto Address { get; set; }
    }
}