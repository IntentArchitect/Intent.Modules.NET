using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AwsLambdaFunction.Application.EfAffiliates;
using AwsLambdaFunction.Application.Interfaces;
using AwsLambdaFunction.Domain.Common.Exceptions;
using AwsLambdaFunction.Domain.Entities;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AwsLambdaFunction.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class EfAffiliatesService : IEfAffiliatesService
    {
        private readonly IEfAffiliateRepository _efAffiliateRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public EfAffiliatesService(IEfAffiliateRepository efAffiliateRepository, IMapper mapper)
        {
            _efAffiliateRepository = efAffiliateRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateEfAffiliate(CreateEfAffiliateDto dto, CancellationToken cancellationToken = default)
        {
            var efAffiliate = new EfAffiliate
            {
                Name = dto.Name
            };

            _efAffiliateRepository.Add(efAffiliate);
            await _efAffiliateRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return efAffiliate.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateEfAffiliate(Guid id, UpdateEfAffiliateDto dto, CancellationToken cancellationToken = default)
        {
            var efAffiliate = await _efAffiliateRepository.FindByIdAsync(id, cancellationToken);
            if (efAffiliate is null)
            {
                throw new NotFoundException($"Could not find EfAffiliate '{id}'");
            }

            efAffiliate.Name = dto.Name;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EfAffiliateDto> FindEfAffiliateById(Guid id, CancellationToken cancellationToken = default)
        {
            var efAffiliate = await _efAffiliateRepository.FindByIdAsync(id, cancellationToken);
            if (efAffiliate is null)
            {
                throw new NotFoundException($"Could not find EfAffiliate '{id}'");
            }
            return efAffiliate.MapToEfAffiliateDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EfAffiliateDto>> FindEfAffiliates(CancellationToken cancellationToken = default)
        {
            var efAffiliates = await _efAffiliateRepository.FindAllAsync(cancellationToken);
            return efAffiliates.MapToEfAffiliateDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteEfAffiliate(Guid id, CancellationToken cancellationToken = default)
        {
            var efAffiliate = await _efAffiliateRepository.FindByIdAsync(id, cancellationToken);
            if (efAffiliate is null)
            {
                throw new NotFoundException($"Could not find EfAffiliate '{id}'");
            }


            _efAffiliateRepository.Remove(efAffiliate);
        }
    }
}