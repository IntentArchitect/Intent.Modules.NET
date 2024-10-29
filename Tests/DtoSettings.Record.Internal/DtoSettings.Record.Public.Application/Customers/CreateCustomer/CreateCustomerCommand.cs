using System;
using DtoSettings.Record.Public.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string surname, DateTime createdDate)
        {
            Name = name;
            Surname = surname;
            CreatedDate = createdDate;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}