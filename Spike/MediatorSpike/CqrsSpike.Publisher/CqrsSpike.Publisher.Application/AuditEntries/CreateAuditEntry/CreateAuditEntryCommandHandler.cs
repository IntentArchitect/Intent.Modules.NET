using CqrsSpike.Publisher.Domain.Entities;
using CqrsSpike.Publisher.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry
{
    public class CreateAuditEntryCommandHandler : ICommandHandler<CreateAuditEntryCommand, Guid>
    {
        private readonly IAuditEntryRepository _auditEntryRepository;

        public CreateAuditEntryCommandHandler(IAuditEntryRepository auditEntryRepository)
        {
            _auditEntryRepository = auditEntryRepository;
        }

        public async ValueTask<Guid> Handle(CreateAuditEntryCommand request, CancellationToken cancellationToken)
        {
            var auditEntry = new AuditEntry
            {
                Description = request.Description,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _auditEntryRepository.Add(auditEntry);
            await _auditEntryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return auditEntry.Id;
        }
    }
}