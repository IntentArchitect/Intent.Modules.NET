using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Products.GetProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public GetProductByIdQueryHandler(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Product.Where(x => x.Id == request.Id).ProjectTo<ProductDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }
            return product;
        }
    }
}