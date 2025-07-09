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
    public class ForShortService : IForShortService
    {
        [IntentManaged(Mode.Merge)]
        public ForShortService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<short> Operation(short param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<short>> OperationCollection(
            List<short> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<short?> OperationNullable(short? param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<short>?> OperationNullableCollection(
            List<short>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}