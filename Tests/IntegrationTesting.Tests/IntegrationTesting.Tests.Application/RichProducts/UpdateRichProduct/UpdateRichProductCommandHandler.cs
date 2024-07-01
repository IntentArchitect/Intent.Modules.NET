using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.UpdateRichProduct
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateRichProductCommandHandler : IRequestHandler<UpdateRichProductCommand>
    {
        private readonly IRichProductRepository _richProductRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateRichProductCommandHandler(IRichProductRepository richProductRepository)
        {
            _richProductRepository = richProductRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateRichProductCommand request, CancellationToken cancellationToken)
        {
            var richProduct = await _richProductRepository.FindByIdAsync(request.Id, cancellationToken);
            if (richProduct is null)
            {
                throw new NotFoundException($"Could not find RichProduct '{request.Id}'");
            }

            richProduct.BrandId = request.BrandId;
            richProduct.Name = request.Name;
        }
    }
}