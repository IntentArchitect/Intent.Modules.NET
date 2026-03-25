using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Interfaces;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.UpdateDocument
{
    public class UpdateDocumentCommand : IRequest, ICommand
    {
        public UpdateDocumentCommand(string id,
            DateTime createdAtUtc,
            DateTime updatedAtUtc,
            DocumentStatus status,
            UpdateDocumentTitleDto title,
            UpdateDocumentContentDto content,
            int revision,
            bool isDeleted,
            List<UpdateDocumentCommandChangesDto> changes,
            List<UpdateDocumentCommandPermissionsDto> permissions,
            List<UpdateDocumentCommandRevisionsDto> revisions,
            List<UpdateDocumentCommandSessionsDto> sessions)
        {
            Id = id;
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

        public string Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DocumentStatus Status { get; set; }
        public UpdateDocumentTitleDto Title { get; set; }
        public UpdateDocumentContentDto Content { get; set; }
        public int Revision { get; set; }
        public bool IsDeleted { get; set; }
        public List<UpdateDocumentCommandChangesDto> Changes { get; set; }
        public List<UpdateDocumentCommandPermissionsDto> Permissions { get; set; }
        public List<UpdateDocumentCommandRevisionsDto> Revisions { get; set; }
        public List<UpdateDocumentCommandSessionsDto> Sessions { get; set; }
    }
}