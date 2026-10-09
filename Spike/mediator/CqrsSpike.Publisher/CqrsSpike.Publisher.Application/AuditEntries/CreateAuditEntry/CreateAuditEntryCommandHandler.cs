using CqrsSpike.Publisher.Domain.Entities;
using CqrsSpike.Publisher.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries.CreateAuditEntry
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAuditEntryCommandHandler : ICommandHandler<CreateAuditEntryCommand, Guid>
    {
        private readonly IAuditEntryRepository _auditEntryRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAuditEntryCommandHandler(IAuditEntryRepository auditEntryRepository)
        {
            _auditEntryRepository = auditEntryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
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