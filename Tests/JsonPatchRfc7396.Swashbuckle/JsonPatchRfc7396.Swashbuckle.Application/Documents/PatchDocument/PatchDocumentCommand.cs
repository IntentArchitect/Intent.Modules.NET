using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Interfaces;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Patching;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.PatchDocument
{
    public class PatchDocumentCommand : IRequest, ICommand, IBypassPipelineValidation
    {
        public PatchDocumentCommand(string id, IPatchExecutor<PatchDocumentCommand> patchExecutor)
        {
            Id = id;
            PatchExecutor = patchExecutor;
        }

        public string Id { get; private set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DocumentStatus Status { get; set; }
        public UpdateDocumentTitleDto Title { get; set; }
        public UpdateDocumentContentDto Content { get; set; }
        public int Revision { get; set; }
        public bool IsDeleted { get; set; }
        public List<PatchDocumentCommandChangesDto> Changes { get; set; }
        public List<PatchDocumentCommandPermissionsDto> Permissions { get; set; }
        public List<PatchDocumentCommandRevisionsDto> Revisions { get; set; }
        public List<PatchDocumentCommandSessionsDto> Sessions { get; set; }
        public IPatchExecutor<PatchDocumentCommand> PatchExecutor { get; private set; }
    }
}