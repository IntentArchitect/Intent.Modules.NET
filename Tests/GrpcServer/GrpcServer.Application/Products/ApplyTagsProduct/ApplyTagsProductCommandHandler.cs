using System;
using System.Threading;
using System.Threading.Tasks;
using GrpcServer.Domain.Common.Exceptions;
using GrpcServer.Domain.Repositories;
using GrpcServer.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GrpcServer.Application.Products.ApplyTagsProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ApplyTagsProductCommandHandler : IRequestHandler<ApplyTagsProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ITagService _tagService;

        [IntentManaged(Mode.Merge)]
        public ApplyTagsProductCommandHandler(IProductRepository productRepository, ITagService tagService)
        {
            _productRepository = productRepository;
            _tagService = tagService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ApplyTagsProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Could not find Product '{request.Id}'");
            }

            await product.ApplyTagsAsync(request.TagNames, _tagService, cancellationToken);
        }
    }
}