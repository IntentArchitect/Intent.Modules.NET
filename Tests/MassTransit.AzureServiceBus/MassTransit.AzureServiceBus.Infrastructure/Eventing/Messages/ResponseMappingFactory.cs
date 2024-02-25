using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Services.RequestResponse.CQRS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.ResponseMappingFactory", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Eventing.Messages
{
    public static class ResponseMappingFactory
    {

        public static object CreateResponseMessage(object originalRequest, object? originalResponse)
        {
            switch (originalRequest)
            {
                case null:
                    throw new ArgumentNullException(nameof(originalRequest));
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandDtoReturn.CommandDtoReturn:
                    return new RequestCompletedMessage<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandResponseDto>(new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandResponseDto((MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandResponseDto)originalResponse));
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandGuidReturn.CommandGuidReturn:
                    return new RequestCompletedMessage<Guid>((Guid)originalResponse);
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandNoParam.CommandNoParam:
                    return RequestCompletedMessage.Instance;
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandVoidReturn.CommandVoidReturn:
                    return RequestCompletedMessage.Instance;
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryGuidReturn.QueryGuidReturn:
                    return new RequestCompletedMessage<Guid>((Guid)originalResponse);
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryNoInputDtoReturnCollection.QueryNoInputDtoReturnCollection:
                    return new RequestCompletedMessage<List<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto>>(new List<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto>(((List<MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDto>)originalResponse).Select(s => new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto(s)).ToList()));
                case MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDtoReturn.QueryResponseDtoReturn:
                    return new RequestCompletedMessage<MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto>(new MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto((MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDto)originalResponse));
                default:
                    throw new ArgumentOutOfRangeException(originalRequest.GetType().Name, "Unexpected request type");
            }
        }
    }
}