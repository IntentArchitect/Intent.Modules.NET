using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Services.RequestResponse;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.ResponseMappingFactory", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Infrastructure.Eventing.Messages
{
    public static class ResponseMappingFactory
    {

        public static object CreateResponseMessage(object originalRequest, object? originalResponse)
        {
            switch (originalRequest)
            {
                case null:
                    throw new ArgumentNullException(nameof(originalRequest));
                case MassTransitFinbuckle.Test.Application.RequestResponse.Test.TestCommand:
                    return RequestCompletedMessage.Instance;
                default:
                    throw new ArgumentOutOfRangeException(originalRequest.GetType().Name, "Unexpected request type");
            }
        }
    }
}