using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Application.Common.Pagination;
using GrpcServer.Application.Interfaces.TypeTestingServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace GrpcServer.Application.Implementation.TypeTestingServices
{
    [IntentManaged(Mode.Merge)]
    public class ForPagedResultService : IForPagedResultService
    {
        [IntentManaged(Mode.Merge)]
        public ForPagedResultService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<ComplexTypeDto>> Operation(
            PagedResult<ComplexTypeDto> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<PagedResult<ComplexTypeDto>>> OperationCollection(
            List<PagedResult<ComplexTypeDto>> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<PagedResult<ComplexTypeDto>> OperationNullable(
            PagedResult<ComplexTypeDto> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<PagedResult<ComplexTypeDto>>> OperationNullableCollection(
            List<PagedResult<ComplexTypeDto>> param,
            CancellationToken cancellationToken = default)
        {
            return param;
        }
    }
}