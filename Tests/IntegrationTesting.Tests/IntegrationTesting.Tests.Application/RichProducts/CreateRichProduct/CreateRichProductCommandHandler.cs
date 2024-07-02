using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.CreateRichProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateRichProductCommandHandler : IRequestHandler<CreateRichProductCommand, Guid>
    {
        private readonly IRichProductRepository _richProductRepository;

        [IntentManaged(Mode.Merge)]
        public CreateRichProductCommandHandler(IRichProductRepository richProductRepository)
        {
            _richProductRepository = richProductRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateRichProductCommand request, CancellationToken cancellationToken)
        {
            var richProduct = new RichProduct(
                brandId: request.BrandId,
                name: request.Name);

            _richProductRepository.Add(richProduct);
            await _richProductRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return richProduct.Id;
        }
    }
}