using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomerStatistics
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerStatisticsQueryHandler : IRequestHandler<GetCustomerStatisticsQuery, int>
    {
        private readonly ICustomerManager _customerManager;

        [IntentManaged(Mode.Merge)]
        public GetCustomerStatisticsQueryHandler(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<int> Handle(GetCustomerStatisticsQuery request, CancellationToken cancellationToken)
        {
            var result = _customerManager.GetCustomerStatistics(request.CustomerId);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}