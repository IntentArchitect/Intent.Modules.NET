using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.ChangeName
{
    public class ChangeNameCommand : IRequest, ICommand
    {
        public ChangeNameCommand(string name, Guid id, string surname)
        {
            Name = name;
            Id = id;
            Surname = surname;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Surname { get; set; }
    }
}