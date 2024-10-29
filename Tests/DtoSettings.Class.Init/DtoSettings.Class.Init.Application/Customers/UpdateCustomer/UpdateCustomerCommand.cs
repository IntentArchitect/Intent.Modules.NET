using System;
using DtoSettings.Class.Init.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(string name, string surname, DateTime createdDate, Guid id)
        {
            Name = name;
            Surname = surname;
            CreatedDate = createdDate;
            Id = id;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid Id { get; set; }
    }
}