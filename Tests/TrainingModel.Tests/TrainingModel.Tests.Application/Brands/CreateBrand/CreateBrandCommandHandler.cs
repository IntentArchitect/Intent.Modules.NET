using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Entities;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Brands.CreateBrand
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Guid>
    {
        private readonly IBrandRepository _brandRepository;

        [IntentManaged(Mode.Merge)]
        public CreateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = new Brand(
                name: request.Name);

            _brandRepository.Add(brand);
            await _brandRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return brand.Id;
        }
    }
}