using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.GetStocks
{
    public class GetStocksQuery : IRequest<List<StockDto>>, IQuery
    {
        public GetStocksQuery()
        {
        }
    }
}