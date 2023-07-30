using System.Collections.Generic;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.GetManyToOneSources
{
    public class GetManyToOneSourcesQuery : IRequest<List<ManyToOneSourceDto>>, IQuery
    {
        public GetManyToOneSourcesQuery()
        {
        }
    }
}