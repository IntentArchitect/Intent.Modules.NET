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
    public class ForStringService : IForStringService
    {
        [IntentManaged(Mode.Merge)]
        public ForStringService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Operation(string param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>> OperationCollection(
            List<string> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string?> OperationNullable(string? param, CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>?> OperationNullableCollection(
            List<string>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}