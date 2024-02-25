using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryNoInputDtoReturnCollection
{
    public class QueryNoInputDtoReturnCollection : IRequest<List<QueryResponseDto>>, IQuery
    {
        public QueryNoInputDtoReturnCollection()
        {
        }
    }
}