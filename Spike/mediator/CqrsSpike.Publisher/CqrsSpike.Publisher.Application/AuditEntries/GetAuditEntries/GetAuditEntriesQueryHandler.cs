using AutoMapper;
using CqrsSpike.Publisher.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAuditEntriesQueryHandler : IQueryHandler<GetAuditEntriesQuery, List<AuditEntryDto>>
    {
        private readonly IAuditEntryRepository _auditEntryRepository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public GetAuditEntriesQueryHandler(IAuditEntryRepository auditEntryRepository, IMapper mapper)
        {
            _auditEntryRepository = auditEntryRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async ValueTask<List<AuditEntryDto>> Handle(GetAuditEntriesQuery request, CancellationToken cancellationToken)
        {
            var auditEntries = await _auditEntryRepository.FindAllAsync(cancellationToken);
            return auditEntries.MapToAuditEntryDtoList(_mapper);
        }
    }
}