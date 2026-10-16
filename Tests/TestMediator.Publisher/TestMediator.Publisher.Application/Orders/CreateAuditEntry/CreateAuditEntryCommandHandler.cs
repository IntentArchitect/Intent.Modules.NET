using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Domain;
using TestMediator.Publisher.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.CreateAuditEntry
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAuditEntryCommandHandler : ICommandHandler<CreateAuditEntryCommand>
    {
        private readonly IAuditEntryRepository _auditEntryRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAuditEntryCommandHandler(IAuditEntryRepository auditEntryRepository)
        {
            _auditEntryRepository = auditEntryRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(CreateAuditEntryCommand request, CancellationToken cancellationToken)
        {
            _auditEntryRepository.Add(new AuditEntry { OrderId = request.OrderId, Message = request.Message });
            await Task.CompletedTask;
            return Unit.Value;
        }
    }
}