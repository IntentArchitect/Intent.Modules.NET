using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Common.Exceptions;
using JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Repositories.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.PatchDocumentChange
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PatchDocumentChangeCommandHandler : IRequestHandler<PatchDocumentChangeCommand, DocumentDocumentChangeDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public PatchDocumentChangeCommandHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DocumentDocumentChangeDto> Handle(
            PatchDocumentChangeCommand request,
            CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindByIdAsync(request.DocumentId, cancellationToken);
            if (document is null)
            {
                throw new NotFoundException($"Could not find Document '{request.DocumentId}'");
            }

            var documentChange = document.Changes.FirstOrDefault(x => x.Id == request.Id);
            if (documentChange is null)
            {
                throw new NotFoundException($"Could not find DocumentChange '{request.Id}'");
            }
            LoadOriginalState(documentChange, request);
            await request.PatchExecutor.ApplyToAsync(request, cancellationToken);
            ApplyChangesTo(request, documentChange);

            _documentRepository.Update(document);
            return documentChange.MapToDocumentDocumentChangeDto(_mapper);
        }

        private static PatchDocumentChangeCommand LoadOriginalState(
            DocumentChange entity,
            PatchDocumentChangeCommand command)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(command);
            command.BaseRevision = entity.BaseRevision;
            command.ResultingRevision = entity.ResultingRevision;
            command.PatchJson = entity.PatchJson;
            command.ChangedAtUtc = entity.ChangedAtUtc;
            command.Actor ??= new PatchDocumentChangeActorDto();
            command.Actor.UserId = entity.Actor.UserId;
            command.Actor.DisplayName = entity.Actor.DisplayName;
            command.ClientChangeId = entity.ClientChangeId;
            return command;
        }

        private static DocumentChange ApplyChangesTo(PatchDocumentChangeCommand command, DocumentChange entity)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(entity);
            entity.BaseRevision = command.BaseRevision;
            entity.ResultingRevision = command.ResultingRevision;
            entity.PatchJson = command.PatchJson;
            entity.ChangedAtUtc = command.ChangedAtUtc;
            entity.Actor = new Actor(
                userId: command.Actor.UserId,
                displayName: command.Actor.DisplayName);
            entity.ClientChangeId = command.ClientChangeId;
            return entity;
        }
    }
}