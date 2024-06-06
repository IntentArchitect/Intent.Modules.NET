using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Common.Exceptions;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Brands.DeactivateBrand
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeactivateBrandCommandHandler : IRequestHandler<DeactivateBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;

        [IntentManaged(Mode.Merge)]
        public DeactivateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeactivateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.FindByIdAsync(request.Id, cancellationToken);
            if (brand is null)
            {
                throw new NotFoundException($"Could not find Brand '{request.Id}'");
            }

            brand.Deactivate();
        }
    }
}