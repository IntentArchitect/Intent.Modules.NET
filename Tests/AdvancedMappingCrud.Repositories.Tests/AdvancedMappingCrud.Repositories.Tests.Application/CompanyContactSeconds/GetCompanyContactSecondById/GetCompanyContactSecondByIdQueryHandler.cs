using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds.GetCompanyContactSecondById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCompanyContactSecondByIdQueryHandler : IRequestHandler<GetCompanyContactSecondByIdQuery, CompanyContactSecondDto>
    {
        private readonly ICompanyContactSecondRepository _companyContactSecondRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCompanyContactSecondByIdQueryHandler(ICompanyContactSecondRepository companyContactSecondRepository,
            IMapper mapper)
        {
            _companyContactSecondRepository = companyContactSecondRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompanyContactSecondDto> Handle(
            GetCompanyContactSecondByIdQuery request,
            CancellationToken cancellationToken)
        {
            var companyContactSecond = await _companyContactSecondRepository.FindByIdAsync(request.Id, cancellationToken);
            if (companyContactSecond is null)
            {
                throw new NotFoundException($"Could not find CompanyContactSecond '{request.Id}'");
            }
            return companyContactSecond.MapToCompanyContactSecondDto(_mapper);
        }
    }
}