using FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.CreateProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        [IntentManaged(Mode.Merge)]
        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Code = request.Code,
                Name = request.Name
            };

            _productRepository.Add(product);
        }
    }
}