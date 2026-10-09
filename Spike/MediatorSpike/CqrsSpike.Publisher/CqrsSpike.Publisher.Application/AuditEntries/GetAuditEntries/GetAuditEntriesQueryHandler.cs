using AutoMapper;
using CqrsSpike.Publisher.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditEntryDto>>
    {
        private readonly IAuditEntryRepository _auditEntryRepository;
        private readonly IMapper _mapper;
        public GetAuditEntriesQueryHandler(IAuditEntryRepository auditEntryRepository, IMapper mapper)
        {
            _auditEntryRepository = auditEntryRepository;
            _mapper = mapper;
        }

        public async ValueTask<List<AuditEntryDto>> Handle(GetAuditEntriesQuery request, CancellationToken cancellationToken)
        {
            var auditEntries = await _auditEntryRepository.FindAllAsync(cancellationToken);
            return auditEntries.MapToAuditEntryDtoList(_mapper);
        }
    }
}