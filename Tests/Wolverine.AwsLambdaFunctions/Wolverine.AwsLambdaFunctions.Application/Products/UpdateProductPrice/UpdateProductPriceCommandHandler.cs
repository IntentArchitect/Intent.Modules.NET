using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AwsLambdaFunctions.Domain.Common.Exceptions;
using Wolverine.AwsLambdaFunctions.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AwsLambdaFunctions.Application.Products.UpdateProductPrice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProductPriceCommandHandler
    {
        private readonly IProductRepository _productRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateProductPriceCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateProductPriceCommand command, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateProductPriceCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}