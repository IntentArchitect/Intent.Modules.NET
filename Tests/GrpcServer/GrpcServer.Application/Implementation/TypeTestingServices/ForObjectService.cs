using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Application.Interfaces.TypeTestingServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GrpcServer.Application.Implementation.TypeTestingServices
{
    [IntentManaged(Mode.Merge)]
    public class ForObjectService : IForObjectService
    {
        [IntentManaged(Mode.Merge)]
        public ForObjectService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<object> Operation(object param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<object>> OperationCollection(
            List<object> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<object?> OperationNullable(object? param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<object>?> OperationNullableCollection(
            List<object>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}