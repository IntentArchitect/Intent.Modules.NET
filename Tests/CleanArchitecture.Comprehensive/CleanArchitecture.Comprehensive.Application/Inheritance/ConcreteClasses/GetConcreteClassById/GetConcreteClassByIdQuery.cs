using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.GetConcreteClassById
{
    public class GetConcreteClassByIdQuery : IRequest<ConcreteClassDto>, IQuery
    {
        public GetConcreteClassByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}