using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.GetStocks
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetStocksQueryHandler : IRequestHandler<GetStocksQuery, List<StockDto>>
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetStocksQueryHandler(IStockRepository stockRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<StockDto>> Handle(GetStocksQuery request, CancellationToken cancellationToken)
        {
            var stocks = await _stockRepository.FindAllAsync(cancellationToken);
            return stocks.MapToStockDtoList(_mapper);
        }
    }
}