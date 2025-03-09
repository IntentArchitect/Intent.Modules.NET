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
    public class ForLongService : IForLongService
    {
        [IntentManaged(Mode.Merge)]
        public ForLongService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<long> Operation(long param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<long>> OperationCollection(List<long> param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<long?> OperationNullable(long? param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<long>?> OperationNullableCollection(
            List<long>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}