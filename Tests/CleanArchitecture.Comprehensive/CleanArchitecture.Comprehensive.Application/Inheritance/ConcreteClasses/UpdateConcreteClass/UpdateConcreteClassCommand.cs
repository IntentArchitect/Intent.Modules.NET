using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.UpdateConcreteClass
{
    public class UpdateConcreteClassCommand : IRequest, ICommand
    {
        public UpdateConcreteClassCommand(Guid id, string concreteAttr, string baseAttr)
        {
            Id = id;
            ConcreteAttr = concreteAttr;
            BaseAttr = baseAttr;
        }

        public Guid Id { get; set; }
        public string ConcreteAttr { get; set; }
        public string BaseAttr { get; set; }
    }
}