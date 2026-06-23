using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AzureFunctions.Domain.Entities;
using Wolverine.AzureFunctions.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Products.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler
    {
        private readonly IProductRepository _productRepository;
        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateProductCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}