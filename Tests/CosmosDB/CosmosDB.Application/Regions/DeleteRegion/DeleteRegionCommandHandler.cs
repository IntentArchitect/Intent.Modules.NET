using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Regions.DeleteRegion
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteRegionCommandHandler : IRequestHandler<DeleteRegionCommand>
    {
        private readonly IRegionRepository _regionRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteRegionCommandHandler(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
        {
            var existingRegion = await _regionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingRegion is null)
            {
                throw new NotFoundException($"Could not find Region '{request.Id}'");
            }

            _regionRepository.Remove(existingRegion);
        }
    }
}