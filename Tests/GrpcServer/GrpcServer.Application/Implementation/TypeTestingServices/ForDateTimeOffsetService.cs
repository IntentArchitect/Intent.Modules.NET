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
    public class ForDateTimeOffsetService : IForDateTimeOffsetService
    {
        [IntentManaged(Mode.Merge)]
        public ForDateTimeOffsetService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<DateTimeOffset> Operation(DateTimeOffset param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<DateTimeOffset>> OperationCollection(
            List<DateTimeOffset> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<DateTimeOffset?> OperationNullable(
            DateTimeOffset? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<DateTimeOffset>?> OperationNullableCollection(
            List<DateTimeOffset>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}