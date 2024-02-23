using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Services.RequestResponse.CQRS;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.ResponseMappingFactory", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Eventing.Messages
{
    public static class ResponseMappingFactory
    {
        private static readonly Dictionary<Type, Func<object, object>> MappingLookup = CreateLookup();

        public static object CreateResponseMessage(object originalResponse)
        {
            var responseType = originalResponse.GetType();
            if (MappingLookup.TryGetValue(responseType, out var predefinedMappingFunc))
            {
                return predefinedMappingFunc(originalResponse);
            }

            return CreateRequestCompletedMessage(originalResponse);
        }

        private static Dictionary<Type, Func<object, object>> CreateLookup()
        {
            var mappingLookup = new Dictionary<Type, Func<object, object>>();
            AddMapping<MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandResponseDto, MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.CommandResponseDto>(mappingLookup);
            AddMapping<MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDto, MassTransit.AzureServiceBus.Services.RequestResponse.CQRS.QueryResponseDto>(mappingLookup);
            return mappingLookup;
        }

        private static void AddMapping<TSource, TDest>(Dictionary<Type, Func<object, object>> mappingLookup)
            where TDest : class
        {
            mappingLookup.Add(typeof(TSource), originalResponse => CreateRequestCompletedMessage(Activator.CreateInstance(typeof(TDest), new[] { originalResponse })!));
        }

        private static object CreateRequestCompletedMessage(object response)
        {
            var responseType = response.GetType();
            var genericType = typeof(RequestCompletedMessage<>).MakeGenericType(responseType);
            var responseInstance = Activator.CreateInstance(genericType, new[] { response })!;
            return responseInstance;
        }
    }
}