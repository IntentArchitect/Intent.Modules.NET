using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Solace.Tests.Application.Customers.GetDapper
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDapperQueryHandler : IRequestHandler<GetDapperQuery, List<CustomerDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetDapperQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(GetDapperQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}