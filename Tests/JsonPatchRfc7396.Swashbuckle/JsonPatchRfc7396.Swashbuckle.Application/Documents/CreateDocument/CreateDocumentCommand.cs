using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Interfaces;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.CreateDocument
{
    public class CreateDocumentCommand : IRequest<string>, ICommand
    {
        public CreateDocumentCommand(DateTime createdAtUtc,
            DateTime updatedAtUtc,
            DocumentStatus status,
            CreateDocumentTitleDto title,
            CreateDocumentContentDto content,
            int revision,
            bool isDeleted,
            List<CreateDocumentCommandChangesDto> changes,
            List<CreateDocumentCommandPermissionsDto> permissions,
            List<CreateDocumentCommandRevisionsDto> revisions,
            List<CreateDocumentCommandSessionsDto> sessions)
        {
            CreatedAtUtc = createdAtUtc;
            UpdatedAtUtc = updatedAtUtc;
            Status = status;
            Title = title;
            Content = content;
            Revision = revision;
            IsDeleted = isDeleted;
            Changes = changes;
            Permissions = permissions;
            Revisions = revisions;
            Sessions = sessions;
        }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DocumentStatus Status { get; set; }
        public CreateDocumentTitleDto Title { get; set; }
        public CreateDocumentContentDto Content { get; set; }
        public int Revision { get; set; }
        public bool IsDeleted { get; set; }
        public List<CreateDocumentCommandChangesDto> Changes { get; set; }
        public List<CreateDocumentCommandPermissionsDto> Permissions { get; set; }
        public List<CreateDocumentCommandRevisionsDto> Revisions { get; set; }
        public List<CreateDocumentCommandSessionsDto> Sessions { get; set; }
    }
}