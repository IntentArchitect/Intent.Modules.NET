using System.Collections.Generic;
using AzureFunctions.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Params.GetByIdsQueryTest
{
    public class GetByIdsQueryTest : IRequest<int>, IQuery
    {
        public GetByIdsQueryTest(List<int> ids)
        {
            Ids = ids;
        }

        public List<int> Ids { get; set; }
    }
}