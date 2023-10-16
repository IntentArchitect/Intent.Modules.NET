using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Regions.UpdateRegion
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand>
    {
        private readonly IRegionRepository _regionRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateRegionCommandHandler(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
        {
            var existingRegion = await _regionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingRegion is null)
            {
                throw new NotFoundException($"Could not find Region '{request.Id}'");
            }

            existingRegion.Name = request.Name;

            _regionRepository.Update(existingRegion);
        }
    }
}