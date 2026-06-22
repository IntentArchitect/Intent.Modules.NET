using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.Products.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler
    {
        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateProductCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}