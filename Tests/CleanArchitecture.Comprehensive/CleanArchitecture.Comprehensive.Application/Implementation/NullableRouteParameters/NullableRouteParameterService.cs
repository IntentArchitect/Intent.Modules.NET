using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Interfaces.NullableRouteParameters;
using CleanArchitecture.Comprehensive.Application.NullableRouteParameters;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Implementation.NullableRouteParameters
{
    [IntentManaged(Mode.Merge)]
    public class NullableRouteParameterService : INullableRouteParameterService
    {
        [IntentManaged(Mode.Merge)]
        public NullableRouteParameterService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task RoutedOperation(
            string? nullableString,
            int? nullableInt,
            NullableRouteParameterEnum? nullableEnum,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement RoutedOperation (NullableRouteParameterService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}