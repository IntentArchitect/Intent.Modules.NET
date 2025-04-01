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

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts.GetCompanyContactById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCompanyContactByIdQueryHandler : IRequestHandler<GetCompanyContactByIdQuery, CompanyContactDto>
    {
        private readonly ICompanyContactRepository _companyContactRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCompanyContactByIdQueryHandler(ICompanyContactRepository companyContactRepository, IMapper mapper)
        {
            _companyContactRepository = companyContactRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CompanyContactDto> Handle(
            GetCompanyContactByIdQuery request,
            CancellationToken cancellationToken)
        {
            var companyContact = await _companyContactRepository.FindByIdAsync(request.Id, cancellationToken);
            if (companyContact is null)
            {
                throw new NotFoundException($"Could not find CompanyContact '{request.Id}'");
            }
            return companyContact.MapToCompanyContactDto(_mapper);
        }
    }
}