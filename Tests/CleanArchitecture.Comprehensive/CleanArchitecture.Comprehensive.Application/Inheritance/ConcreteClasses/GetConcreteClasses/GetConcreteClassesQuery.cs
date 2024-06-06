using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.GetConcreteClasses
{
    public class GetConcreteClassesQuery : IRequest<List<ConcreteClassDto>>, IQuery
    {
        public GetConcreteClassesQuery()
        {
        }
    }
}