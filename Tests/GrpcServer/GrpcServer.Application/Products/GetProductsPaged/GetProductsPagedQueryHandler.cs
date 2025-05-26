using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GrpcServer.Application.Common.Pagination;
using GrpcServer.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace GrpcServer.Application.Products.GetProductsPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsPagedQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ProductDto>> Handle(
            GetProductsPagedQuery request,
            CancellationToken cancellationToken)
        {
            var products = await _productRepository.FindAllAsync(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return products.MapToPagedResult(x => x.MapToProductDto(_mapper));
        }
    }
}