using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities.GetTestEntities
{
    public class GetTestEntitiesQuery : IRequest<List<TestEntityDto>>, IQuery
    {
        public GetTestEntitiesQuery()
        {
        }
    }
}