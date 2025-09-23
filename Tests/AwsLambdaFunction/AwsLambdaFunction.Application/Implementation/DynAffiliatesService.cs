using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AwsLambdaFunction.Application.DynAffiliates;
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
    public class DynAffiliatesService : IDynAffiliatesService
    {
        private readonly IDynAffiliateRepository _dynAffiliateRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public DynAffiliatesService(IDynAffiliateRepository dynAffiliateRepository, IMapper mapper)
        {
            _dynAffiliateRepository = dynAffiliateRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> CreateDynAffiliate(
            CreateDynAffiliateDto dto,
            CancellationToken cancellationToken = default)
        {
            var dynAffiliate = new DynAffiliate
            {
                Name = dto.Name
            };

            _dynAffiliateRepository.Add(dynAffiliate);
            await _dynAffiliateRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return dynAffiliate.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateDynAffiliate(
            string id,
            UpdateDynAffiliateDto dto,
            CancellationToken cancellationToken = default)
        {
            var dynAffiliate = await _dynAffiliateRepository.FindByIdAsync(id, cancellationToken);
            if (dynAffiliate is null)
            {
                throw new NotFoundException($"Could not find DynAffiliate '{id}'");
            }

            dynAffiliate.Name = dto.Name;

            _dynAffiliateRepository.Update(dynAffiliate);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DynAffiliateDto> FindAffiliateById(string id, CancellationToken cancellationToken = default)
        {
            var dynAffiliate = await _dynAffiliateRepository.FindByIdAsync(id, cancellationToken);
            if (dynAffiliate is null)
            {
                throw new NotFoundException($"Could not find DynAffiliate '{id}'");
            }
            return dynAffiliate.MapToDynAffiliateDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DynAffiliateDto>> FindDynAffiliates(CancellationToken cancellationToken = default)
        {
            var dynAffiliates = await _dynAffiliateRepository.FindAllAsync(cancellationToken);
            return dynAffiliates.MapToDynAffiliateDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteDynAffiliate(string id, CancellationToken cancellationToken = default)
        {
            var dynAffiliate = await _dynAffiliateRepository.FindByIdAsync(id, cancellationToken);
            if (dynAffiliate is null)
            {
                throw new NotFoundException($"Could not find DynAffiliate '{id}'");
            }


            _dynAffiliateRepository.Remove(dynAffiliate);
        }
    }
}