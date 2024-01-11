using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.GetProductsPaginatedByNameOptional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsPaginatedByNameOptionalQueryHandler : IRequestHandler<GetProductsPaginatedByNameOptionalQuery, PagedResult<ProductDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetProductsPaginatedByNameOptionalQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<ProductDto>> Handle(
            GetProductsPaginatedByNameOptionalQuery request,
            CancellationToken cancellationToken)
        {
            var queryable = _dbContext.Products.AsQueryable();

            if (request.Name != null)
            {
                queryable = queryable.Where(x => x.Name == request.Name);
            }

            var products = await queryable
                .ToPagedListAsync(request.PageNo, request.PageSize, cancellationToken);
            return products.MapToPagedResult(x => x.MapToProductDto(_mapper));
        }
    }
}