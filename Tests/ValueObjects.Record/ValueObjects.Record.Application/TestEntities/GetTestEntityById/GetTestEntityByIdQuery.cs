using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities.GetTestEntityById
{
    public class GetTestEntityByIdQuery : IRequest<TestEntityDto>, IQuery
    {
        public GetTestEntityByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}