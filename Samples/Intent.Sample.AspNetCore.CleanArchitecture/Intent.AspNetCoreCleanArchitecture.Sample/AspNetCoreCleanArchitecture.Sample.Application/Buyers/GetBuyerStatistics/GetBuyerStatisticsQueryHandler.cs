using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyerStatistics
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetBuyerStatisticsQueryHandler : IRequestHandler<GetBuyerStatisticsQuery, BuyerStatisticsDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetBuyerStatisticsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<BuyerStatisticsDto> Handle(GetBuyerStatisticsQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}