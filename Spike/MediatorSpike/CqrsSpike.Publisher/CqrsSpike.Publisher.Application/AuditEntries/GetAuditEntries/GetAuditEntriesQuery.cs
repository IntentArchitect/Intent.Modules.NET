using Mediator;

namespace CqrsSpike.Publisher.Application.AuditEntries.GetAuditEntries
{
    public class GetAuditEntriesQuery : IQuery<List<AuditEntryDto>>
    {
        public GetAuditEntriesQuery()
        {
        }
    }
}