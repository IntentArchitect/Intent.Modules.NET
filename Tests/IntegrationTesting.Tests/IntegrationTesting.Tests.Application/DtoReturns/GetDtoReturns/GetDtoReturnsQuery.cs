using System.Collections.Generic;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.GetDtoReturns
{
    public class GetDtoReturnsQuery : IRequest<List<DtoReturnDto>>, IQuery
    {
        public GetDtoReturnsQuery()
        {
        }
    }
}