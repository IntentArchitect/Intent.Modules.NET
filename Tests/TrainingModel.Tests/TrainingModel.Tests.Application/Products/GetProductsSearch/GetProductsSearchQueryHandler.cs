using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace TrainingModel.Tests.Application.Products.GetProductsSearch
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsSearchQueryHandler : IRequestHandler<GetProductsSearchQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsSearchQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ProductDto>> Handle(GetProductsSearchQuery request, CancellationToken cancellationToken)
        {
            //IntentIgnore
            var products = await _productRepository.FindAllAsync(
                p => p.IsActive && (
                    p.Name.Contains(request.SearchTerm) ||
                    p.Categories.Any(c => c.Name.Contains(request.SearchTerm))),
                    cancellationToken);
            return products.MapToProductDtoList(_mapper);
        }
    }
}