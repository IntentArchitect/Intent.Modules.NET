using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DomainServiceTests.GetDomainServiceTestById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDomainServiceTestByIdQueryHandler : IRequestHandler<GetDomainServiceTestByIdQuery, DomainServiceTestDto>
    {
        private readonly IDomainServiceTestRepository _domainServiceTestRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDomainServiceTestByIdQueryHandler(IDomainServiceTestRepository domainServiceTestRepository,
            IMapper mapper)
        {
            _domainServiceTestRepository = domainServiceTestRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DomainServiceTestDto> Handle(
            GetDomainServiceTestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var domainServiceTest = await _domainServiceTestRepository.FindByIdAsync(request.Id, cancellationToken);
            if (domainServiceTest is null)
            {
                throw new NotFoundException($"Could not find DomainServiceTest '{request.Id}'");
            }

            return domainServiceTest.MapToDomainServiceTestDto(_mapper);
        }
    }
}