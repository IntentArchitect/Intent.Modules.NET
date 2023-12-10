using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCusomterStatistics
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class GetCusomterStatisticsQueryHandler : IRequestHandler<GetCusomterStatisticsQuery, int>
    {
        private readonly ICustomerManager _customerManager;

        [IntentManaged(Mode.Fully)]
        public GetCusomterStatisticsQueryHandler(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<int> Handle(GetCusomterStatisticsQuery request, CancellationToken cancellationToken)
        {
            var result = _customerManager.GetCustomerStatistics(request.CustomerId);

            // [IntentIgnore(Match = "throw")]
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}