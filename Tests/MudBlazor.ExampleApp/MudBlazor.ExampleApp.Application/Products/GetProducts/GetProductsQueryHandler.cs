using System.Linq.Dynamic.Core;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Pagination;
using MudBlazor.ExampleApp.Domain.Entities;
using MudBlazor.ExampleApp.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Products.GetProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Common.Pagination.PagedResult<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<ProductDto>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> FilterProducts(IQueryable<Product> queryable)
            {
                if (request.SearchText != null)
                {
                    queryable = queryable.Where(x => x.Name == request.SearchText);
                }
                return queryable;
            }

            var products = await _productRepository.FindAllAsync(request.PageNo, request.PageSize, q => FilterProducts(q).OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return products.MapToPagedResult(x => x.MapToProductDto(_mapper));
        }
    }
}