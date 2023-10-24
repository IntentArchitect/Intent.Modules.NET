using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Regions.CreateRegion
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateRegionCommandHandler : IRequestHandler<CreateRegionCommand, string>
    {
        private readonly IRegionRepository _regionRepository;

        [IntentManaged(Mode.Merge)]
        public CreateRegionCommandHandler(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
        {
            var newRegion = new Region
            {
                Name = request.Name,
                Countries = request.Countries.Select(CreateCountry).ToList(),
            };

            _regionRepository.Add(newRegion);
            await _regionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newRegion.Id;
        }

        [IntentManaged(Mode.Fully)]
        private static Country CreateCountry(CreateRegionCountryDto dto)
        {
            return new Country
            {
                Name = dto.Name,
            };
        }
    }
}