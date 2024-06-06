using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.DeleteConcreteClass
{
    public class DeleteConcreteClassCommand : IRequest, ICommand
    {
        public DeleteConcreteClassCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}