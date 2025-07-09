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
    public class ForDictionaryService : IForDictionaryService
    {
        [IntentManaged(Mode.Merge)]
        public ForDictionaryService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Dictionary<string, string>> Operation(
            Dictionary<string, string> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<Dictionary<string, string>>> OperationCollection(
            List<Dictionary<string, string>> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Dictionary<string, string>?> OperationNullable(
            Dictionary<string, string>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<Dictionary<string, string>>?> OperationNullableCollection(
            List<Dictionary<string, string>>? param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}