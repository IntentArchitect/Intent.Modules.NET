using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Common;
using JsonPatchRfc7396.Scalar.Domain.Common.Exceptions;
using JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Repositories.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.UpdateDocument
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand>
    {
        private readonly IDocumentRepository _documentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDocumentCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (document is null)
            {
                throw new NotFoundException($"Could not find Document '{request.Id}'");
            }

            document.CreatedAtUtc = request.CreatedAtUtc;
            document.UpdatedAtUtc = request.UpdatedAtUtc;
            document.Status = request.Status;
            document.Title = new DocumentTitle(
                value: request.Title.Value);
            document.Content = new DocumentContent(
                format: request.Content.Format,
                text: request.Content.Text,
                json: request.Content.Json);
            document.Revision = request.Revision;
            document.IsDeleted = request.IsDeleted;
            document.Revisions = UpdateHelper.CreateOrUpdateCollection(document.Revisions, request.Revisions, (e, d) => e.Id == d.Id, CreateOrUpdateDocumentRevision);
            document.Changes = UpdateHelper.CreateOrUpdateCollection(document.Changes, request.Changes, (e, d) => e.Id == d.Id, CreateOrUpdateDocumentChange);
            document.Sessions = UpdateHelper.CreateOrUpdateCollection(document.Sessions, request.Sessions, (e, d) => e.Id == d.Id, CreateOrUpdateCollaboratorSession);
            document.Permissions = UpdateHelper.CreateOrUpdateCollection(document.Permissions, request.Permissions, (e, d) => e.Id == d.Id, CreateOrUpdateDocumentPermission);

            _documentRepository.Update(document);
        }

        [IntentManaged(Mode.Fully)]
        private static DocumentRevision CreateOrUpdateDocumentRevision(
            DocumentRevision? entity,
            UpdateDocumentCommandRevisionsDto dto)
        {
            entity ??= new DocumentRevision();
            entity.Id = dto.Id;
            entity.Revision = dto.Revision;
            entity.CreatedAtUtc = dto.CreatedAtUtc;
            entity.Content = new DocumentContent(
                format: dto.Content.Format,
                text: dto.Content.Text,
                json: dto.Content.Json);
            entity.Author = new Actor(
                userId: dto.Author.UserId,
                displayName: dto.Author.DisplayName);
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static DocumentChange CreateOrUpdateDocumentChange(
            DocumentChange? entity,
            UpdateDocumentCommandChangesDto dto)
        {
            entity ??= new DocumentChange();
            entity.Id = dto.Id;
            entity.BaseRevision = dto.BaseRevision;
            entity.ResultingRevision = dto.ResultingRevision;
            entity.PatchJson = dto.PatchJson;
            entity.ChangedAtUtc = dto.ChangedAtUtc;
            entity.Actor = new Actor(
                userId: dto.Actor.UserId,
                displayName: dto.Actor.DisplayName);
            entity.ClientChangeId = dto.ClientChangeId;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static CollaboratorSession CreateOrUpdateCollaboratorSession(
            CollaboratorSession? entity,
            UpdateDocumentCommandSessionsDto dto)
        {
            entity ??= new CollaboratorSession();
            entity.Id = dto.Id;
            entity.ConnectionId = dto.ConnectionId;
            entity.StartedAtUtc = dto.StartedAtUtc;
            entity.LastSeenAtUtc = dto.LastSeenAtUtc;
            entity.CursorJson = dto.CursorJson;
            entity.SelectionJson = dto.SelectionJson;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static DocumentPermission CreateOrUpdateDocumentPermission(
            DocumentPermission? entity,
            UpdateDocumentCommandPermissionsDto dto)
        {
            entity ??= new DocumentPermission();
            entity.Id = dto.Id;
            entity.Role = dto.Role;
            entity.PrincipalId = dto.PrincipalId;
            entity.PrincipalType = dto.PrincipalType;
            entity.GrantedAtUtc = dto.GrantedAtUtc;
            return entity;
        }
    }
}