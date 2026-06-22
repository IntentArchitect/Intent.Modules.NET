using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AzureFunctions.Application.Products.UpdateProductPrice
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateProductPriceCommandHandler
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProductPriceCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(UpdateProductPriceCommand command, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateProductPriceCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}