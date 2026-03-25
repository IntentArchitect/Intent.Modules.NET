using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Common;
using JsonPatchRfc7396.Scalar.Domain.Common.Exceptions;
using JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Repositories.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.PatchDocument
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PatchDocumentCommandHandler : IRequestHandler<PatchDocumentCommand, DocumentDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public PatchDocumentCommandHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DocumentDto> Handle(PatchDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (document is null)
            {
                throw new NotFoundException($"Could not find Document '{request.Id}'");
            }

            LoadOriginalState(document, request);
            request.PatchExecutor.ApplyTo(request);
            ApplyChangesTo(request, document);

            _documentRepository.Update(document);
            return document.MapToDocumentDto(_mapper);
        }

        private static PatchDocumentCommand LoadOriginalState(Document entity, PatchDocumentCommand command)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(command);
            command.CreatedAtUtc = entity.CreatedAtUtc;
            command.UpdatedAtUtc = entity.UpdatedAtUtc;
            command.Status = entity.Status;
            command.Title ??= new PatchDocumentCommandTitleDto();
            command.Title.Value = entity.Title.Value;
            command.Content ??= new PatchDocumentCommandContentDto();
            command.Content.Format = entity.Content.Format;
            command.Content.Text = entity.Content.Text;
            command.Content.Json = entity.Content.Json;
            command.Revision = entity.Revision;
            command.IsDeleted = entity.IsDeleted;
            command.Changes = entity.Changes
                .Select(c => new PatchDocumentCommandChangesDto
                {
                    Id = c.Id,
                    BaseRevision = c.BaseRevision,
                    ResultingRevision = c.ResultingRevision,
                    PatchJson = c.PatchJson,
                    ChangedAtUtc = c.ChangedAtUtc,
                    Actor = new PatchDocumentCommandActorDto
                    {
                        UserId = c.Actor.UserId,
                        DisplayName = c.Actor.DisplayName
                    },
                    ClientChangeId = c.ClientChangeId
                })
                .ToList();
            command.Permissions = entity.Permissions
                .Select(p => new PatchDocumentCommandPermissionsDto
                {
                    Id = p.Id,
                    Role = p.Role,
                    PrincipalId = p.PrincipalId,
                    PrincipalType = p.PrincipalType,
                    GrantedAtUtc = p.GrantedAtUtc
                })
                .ToList();
            command.Revisions = entity.Revisions
                .Select(r => new PatchDocumentCommandRevisionsDto
                {
                    Id = r.Id,
                    Revision = r.Revision,
                    CreatedAtUtc = r.CreatedAtUtc,
                    Content = new PatchDocumentCommandContentDto
                    {
                        Format = r.Content.Format,
                        Text = r.Content.Text,
                        Json = r.Content.Json
                    },
                    Author = new PatchDocumentCommandActorDto
                    {
                        UserId = r.Author.UserId,
                        DisplayName = r.Author.DisplayName
                    }
                })
                .ToList();
            command.Sessions = entity.Sessions
                .Select(s => new PatchDocumentCommandSessionsDto
                {
                    Id = s.Id,
                    ConnectionId = s.ConnectionId,
                    StartedAtUtc = s.StartedAtUtc,
                    LastSeenAtUtc = s.LastSeenAtUtc,
                    CursorJson = s.CursorJson,
                    SelectionJson = s.SelectionJson
                })
                .ToList();
            return command;
        }

        private static Document ApplyChangesTo(PatchDocumentCommand command, Document entity)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(entity);
            entity.CreatedAtUtc = command.CreatedAtUtc;
            entity.UpdatedAtUtc = command.UpdatedAtUtc;
            entity.Status = command.Status;
            entity.Title = new DocumentTitle(
                value: command.Title.Value);
            entity.Content = new DocumentContent(
                format: command.Content.Format,
                text: command.Content.Text,
                json: command.Content.Json);
            entity.Revision = command.Revision;
            entity.IsDeleted = command.IsDeleted;
            entity.Revisions = UpdateHelper.CreateOrUpdateCollection(entity.Revisions, command.Revisions, (e, d) => e.Id == d.Id, CreateOrUpdateDocumentRevision);
            entity.Changes = UpdateHelper.CreateOrUpdateCollection(entity.Changes, command.Changes, (e, d) => e.Id == d.Id, CreateOrUpdateDocumentChange);
            entity.Sessions = UpdateHelper.CreateOrUpdateCollection(entity.Sessions, command.Sessions, (e, d) => e.Id == d.Id, CreateOrUpdateCollaboratorSession);
            entity.Permissions = UpdateHelper.CreateOrUpdateCollection(entity.Permissions, command.Permissions, (e, d) => e.Id == d.Id, CreateOrUpdateDocumentPermission);
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static DocumentRevision CreateOrUpdateDocumentRevision(
            DocumentRevision? entity,
            PatchDocumentCommandRevisionsDto dto)
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
            PatchDocumentCommandChangesDto dto)
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
            PatchDocumentCommandSessionsDto dto)
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
            PatchDocumentCommandPermissionsDto dto)
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