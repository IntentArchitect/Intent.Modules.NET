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
    public class ForDoubleService : IForDoubleService
    {
        [IntentManaged(Mode.Merge)]
        public ForDoubleService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<double> Operation(double param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<double>> OperationCollection(
            List<double> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<double?> OperationNullable(double? param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<double>?> OperationNullableCollection(
            List<double>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}