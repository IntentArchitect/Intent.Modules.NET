using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.GetTestNullablityById
{
    public class GetTestNullablityByIdQuery : IRequest<TestNullablityDto>, IQuery
    {
        public GetTestNullablityByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}