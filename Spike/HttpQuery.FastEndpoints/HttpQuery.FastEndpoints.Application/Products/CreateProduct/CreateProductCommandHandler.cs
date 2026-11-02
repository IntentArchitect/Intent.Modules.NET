using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace HttpQuery.FastEndpoints.Application.Products.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateProductCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}