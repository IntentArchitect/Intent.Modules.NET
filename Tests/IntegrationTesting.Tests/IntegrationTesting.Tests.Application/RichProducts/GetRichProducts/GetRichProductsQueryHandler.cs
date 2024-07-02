using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.GetRichProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetRichProductsQueryHandler : IRequestHandler<GetRichProductsQuery, List<RichProductDto>>
    {
        private readonly IRichProductRepository _richProductRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetRichProductsQueryHandler(IRichProductRepository richProductRepository, IMapper mapper)
        {
            _richProductRepository = richProductRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<RichProductDto>> Handle(GetRichProductsQuery request, CancellationToken cancellationToken)
        {
            var richProducts = await _richProductRepository.FindAllAsync(cancellationToken);
            return richProducts.MapToRichProductDtoList(_mapper);
        }
    }
}