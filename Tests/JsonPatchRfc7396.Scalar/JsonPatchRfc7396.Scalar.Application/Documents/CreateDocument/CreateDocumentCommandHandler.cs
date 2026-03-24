using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing;
using JsonPatchRfc7396.Scalar.Domain.Repositories.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.CreateDocument
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, string>
    {
        private readonly IDocumentRepository _documentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDocumentCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = new Document
            {
                CreatedAtUtc = request.CreatedAtUtc,
                UpdatedAtUtc = request.UpdatedAtUtc,
                Status = request.Status,
                Title = new DocumentTitle(
                    value: request.Title.Value),
                Content = new DocumentContent(
                    format: request.Content.Format,
                    text: request.Content.Text,
                    json: request.Content.Json),
                Revision = request.Revision,
                IsDeleted = request.IsDeleted,
                Revisions = request.Revisions
                    .Select(r => new DocumentRevision
                    {
                        Revision = r.Revision,
                        CreatedAtUtc = r.CreatedAtUtc,
                        Content = new DocumentContent(
                            format: r.Content.Format,
                            text: r.Content.Text,
                            json: r.Content.Json),
                        Author = new Actor(
                            userId: r.Author.UserId,
                            displayName: r.Author.DisplayName)
                    })
                    .ToList(),
                Changes = request.Changes
                    .Select(c => new DocumentChange
                    {
                        BaseRevision = c.BaseRevision,
                        ResultingRevision = c.ResultingRevision,
                        PatchJson = c.PatchJson,
                        ChangedAtUtc = c.ChangedAtUtc,
                        Actor = new Actor(
                            userId: c.Actor.UserId,
                            displayName: c.Actor.DisplayName),
                        ClientChangeId = c.ClientChangeId
                    })
                    .ToList(),
                Sessions = request.Sessions
                    .Select(s => new CollaboratorSession
                    {
                        ConnectionId = s.ConnectionId,
                        StartedAtUtc = s.StartedAtUtc,
                        LastSeenAtUtc = s.LastSeenAtUtc,
                        CursorJson = s.CursorJson,
                        SelectionJson = s.SelectionJson
                    })
                    .ToList(),
                Permissions = request.Permissions
                    .Select(p => new DocumentPermission
                    {
                        Role = p.Role,
                        PrincipalId = p.PrincipalId,
                        PrincipalType = p.PrincipalType,
                        GrantedAtUtc = p.GrantedAtUtc
                    })
                    .ToList()
            };

            _documentRepository.Add(document);
            await _documentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return document.Id;
        }
    }
}