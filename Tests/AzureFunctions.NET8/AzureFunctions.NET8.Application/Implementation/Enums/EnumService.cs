using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Enums;
using AzureFunctions.NET8.Application.Interfaces.Enums;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Implementation.Enums
{
    [IntentManaged(Mode.Merge)]
    public class EnumService : IEnumService
    {
        [IntentManaged(Mode.Merge)]
        public EnumService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task TestRouteEnum(Company testEnum, CancellationToken cancellationToken = default)
        {
            // TODO: Implement TestRouteEnum (EnumService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task TestQueryEnum(Company testEnum, CancellationToken cancellationToken = default)
        {
            // TODO: Implement TestQueryEnum (EnumService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task TestHeaderEnum(Company testEnum, CancellationToken cancellationToken = default)
        {
            // TODO: Implement TestHeaderEnum (EnumService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}