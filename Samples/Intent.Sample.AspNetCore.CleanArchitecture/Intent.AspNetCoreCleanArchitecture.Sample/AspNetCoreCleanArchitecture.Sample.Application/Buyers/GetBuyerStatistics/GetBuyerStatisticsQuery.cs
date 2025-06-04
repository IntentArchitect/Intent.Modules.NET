using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.GetBuyerStatistics
{
    public class GetBuyerStatisticsQuery : IRequest<BuyerStatisticsDto>, IQuery
    {
        public GetBuyerStatisticsQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}