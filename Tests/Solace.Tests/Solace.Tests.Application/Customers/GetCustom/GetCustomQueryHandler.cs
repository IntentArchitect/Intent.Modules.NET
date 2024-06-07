using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Solace.Tests.Application.Customers.GetCustom
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomQueryHandler : IRequestHandler<GetCustomQuery, List<CustomerCustomDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerCustomDto>> Handle(GetCustomQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}