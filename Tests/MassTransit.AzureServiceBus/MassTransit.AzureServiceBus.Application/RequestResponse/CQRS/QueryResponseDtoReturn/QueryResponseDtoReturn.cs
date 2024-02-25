using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDtoReturn
{
    public class QueryResponseDtoReturn : IRequest<QueryResponseDto>, IQuery
    {
        public QueryResponseDtoReturn(string input)
        {
            Input = input;
        }

        public string Input { get; set; }
    }
}