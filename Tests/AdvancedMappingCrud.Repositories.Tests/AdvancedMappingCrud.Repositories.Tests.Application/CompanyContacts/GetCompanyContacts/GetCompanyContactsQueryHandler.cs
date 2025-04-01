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

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.GetCompanyContacts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCompanyContactsQueryHandler : IRequestHandler<GetCompanyContactsQuery, List<CompanyContactDto>>
    {
        private readonly ICompanyContactRepository _companyContactRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCompanyContactsQueryHandler(ICompanyContactRepository companyContactRepository, IMapper mapper)
        {
            _companyContactRepository = companyContactRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CompanyContactDto>> Handle(
            GetCompanyContactsQuery request,
            CancellationToken cancellationToken)
        {
            var companyContacts = await _companyContactRepository.FindAllAsync(cancellationToken);
            return companyContacts.MapToCompanyContactDtoList(_mapper);
        }
    }
}