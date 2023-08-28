using System.Collections.Generic;
using AzureFunctions.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Params.GetByIdsHeadersTest
{
    public class GetByIdsHeadersTest : IRequest<int>, IQuery
    {
        public GetByIdsHeadersTest(List<int> ids)
        {
            Ids = ids;
        }

        public List<int> Ids { get; set; }
    }
}