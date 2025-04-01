using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.GetCompanyContactSeconds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCompanyContactSecondsQueryHandler : IRequestHandler<GetCompanyContactSecondsQuery, List<CompanyContactSecondDto>>
    {
        private readonly ICompanyContactSecondRepository _companyContactSecondRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCompanyContactSecondsQueryHandler(ICompanyContactSecondRepository companyContactSecondRepository,
            IMapper mapper)
        {
            _companyContactSecondRepository = companyContactSecondRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompanyContactSecondDto>> Handle(
            GetCompanyContactSecondsQuery request,
            CancellationToken cancellationToken)
        {
            var companyContactSeconds = await _companyContactSecondRepository.FindAllAsync(cancellationToken);
            return companyContactSeconds.MapToCompanyContactSecondDtoList(_mapper);
        }
    }
}