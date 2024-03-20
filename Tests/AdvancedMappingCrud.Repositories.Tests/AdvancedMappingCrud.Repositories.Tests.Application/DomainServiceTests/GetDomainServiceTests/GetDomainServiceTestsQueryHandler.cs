using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.GetDomainServiceTests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDomainServiceTestsQueryHandler : IRequestHandler<GetDomainServiceTestsQuery, List<DomainServiceTestDto>>
    {
        private readonly IDomainServiceTestRepository _domainServiceTestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDomainServiceTestsQueryHandler(IDomainServiceTestRepository domainServiceTestRepository, IMapper mapper)
        {
            _domainServiceTestRepository = domainServiceTestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DomainServiceTestDto>> Handle(
            GetDomainServiceTestsQuery request,
            CancellationToken cancellationToken)
        {
            var domainServiceTests = await _domainServiceTestRepository.FindAllAsync(cancellationToken);
            return domainServiceTests.MapToDomainServiceTestDtoList(_mapper);
        }
    }
}