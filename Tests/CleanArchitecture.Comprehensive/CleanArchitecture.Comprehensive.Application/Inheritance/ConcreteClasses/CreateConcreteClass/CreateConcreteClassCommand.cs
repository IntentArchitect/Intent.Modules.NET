using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.CreateConcreteClass
{
    public class CreateConcreteClassCommand : IRequest<Guid>, ICommand
    {
        public CreateConcreteClassCommand(string concreteAttr, string baseAttr)
        {
            ConcreteAttr = concreteAttr;
            BaseAttr = baseAttr;
        }

        public string ConcreteAttr { get; set; }
        public string BaseAttr { get; set; }
    }
}