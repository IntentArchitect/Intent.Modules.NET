using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.EnumRoute;
using AzureFunctions.TestApplication.Application.Interfaces.EnumRoute;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation.EnumRoute
{
    [IntentManaged(Mode.Merge)]
    public class RouteEnumService : IRouteEnumService
    {
        [IntentManaged(Mode.Merge)]
        public RouteEnumService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task TestRouteEnum(Company testEnum, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}