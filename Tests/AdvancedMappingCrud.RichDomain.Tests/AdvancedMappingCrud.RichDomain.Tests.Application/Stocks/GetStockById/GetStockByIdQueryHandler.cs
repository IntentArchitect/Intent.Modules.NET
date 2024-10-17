using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Stocks.GetStockById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetStockByIdQueryHandler : IRequestHandler<GetStockByIdQuery, StockDto>
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetStockByIdQueryHandler(IStockRepository stockRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<StockDto> Handle(GetStockByIdQuery request, CancellationToken cancellationToken)
        {
            var stock = await _stockRepository.FindByIdAsync(request.Id, cancellationToken);
            if (stock is null)
            {
                throw new NotFoundException($"Could not find Stock '{request.Id}'");
            }
            return stock.MapToStockDto(_mapper);
        }
    }
}