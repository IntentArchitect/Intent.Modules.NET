using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(string name,
            string surname,
            string email,
            bool isActive,
            Guid id,
            List<UpdateCustomerCommandAddressDto> address,
            bool specials,
            bool news)
        {
            Name = name;
            Surname = surname;
            Email = email;
            IsActive = isActive;
            Id = id;
            Address = address;
            Specials = specials;
            News = news;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public Guid Id { get; set; }
        public List<UpdateCustomerCommandAddressDto> Address { get; set; }
        public bool Specials { get; set; }
        public bool News { get; set; }
    }
}