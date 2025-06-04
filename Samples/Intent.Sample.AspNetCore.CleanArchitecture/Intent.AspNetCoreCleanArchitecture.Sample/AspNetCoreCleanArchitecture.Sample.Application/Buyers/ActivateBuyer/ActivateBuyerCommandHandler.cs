using AspNetCoreCleanArchitecture.Sample.Domain.Common.Exceptions;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.ActivateBuyer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ActivateBuyerCommandHandler : IRequestHandler<ActivateBuyerCommand>
    {
        private readonly IBuyerRepository _buyerRepository;

        [IntentManaged(Mode.Merge)]
        public ActivateBuyerCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ActivateBuyerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _buyerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Buyer '{request.Id}'");
            }

            entity.IsActive = request.IsActive;

        }
    }
}