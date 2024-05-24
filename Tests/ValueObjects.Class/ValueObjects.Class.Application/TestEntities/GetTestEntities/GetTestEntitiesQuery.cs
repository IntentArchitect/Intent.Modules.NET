using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Class.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ValueObjects.Class.Application.TestEntities.GetTestEntities
{
    public class GetTestEntitiesQuery : IRequest<List<TestEntityDto>>, IQuery
    {
        public GetTestEntitiesQuery()
        {
        }
    }
}