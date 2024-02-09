using System.Collections.Generic;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.BadSignatures.GetBadSignatures
{
    public class GetBadSignaturesQuery : IRequest<List<BadSignaturesDto>>, IQuery
    {
        public GetBadSignaturesQuery(string filter)
        {
            Filter = filter;
        }

        public string Filter { get; set; }
    }
}