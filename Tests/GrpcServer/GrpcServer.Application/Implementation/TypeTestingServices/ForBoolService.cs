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
    public class ForBoolService : IForBoolService
    {
        [IntentManaged(Mode.Merge)]
        public ForBoolService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<bool> Operation(bool param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<bool>> OperationCollection(List<bool> param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<bool?> OperationNullable(bool? param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<bool>?> OperationNullableCollection(
            List<bool>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}