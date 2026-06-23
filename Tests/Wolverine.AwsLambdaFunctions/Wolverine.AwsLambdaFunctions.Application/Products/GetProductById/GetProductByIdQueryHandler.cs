using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AwsLambdaFunctions.Domain.Common.Exceptions;
using Wolverine.AwsLambdaFunctions.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Application.Products.GetProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductByIdQueryHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task<ProductDto> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetProductByIdQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}