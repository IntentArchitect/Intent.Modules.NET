using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Interfaces;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Patching;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.PatchDocumentChange
{
    public class PatchDocumentChangeCommand : IRequest<DocumentDocumentChangeDto>, ICommand, IBypassPipelineValidation
    {
        public PatchDocumentChangeCommand(string documentId,
            string id,
            IPatchExecutor<PatchDocumentChangeCommand> patchExecutor)
        {
            DocumentId = documentId;
            Id = id;
            PatchExecutor = patchExecutor;
        }

        public string DocumentId { get; private set; }
        public string Id { get; private set; }
        public int BaseRevision { get; set; }
        public int ResultingRevision { get; set; }
        public string PatchJson { get; set; }
        public DateTime ChangedAtUtc { get; set; }
        public PatchDocumentChangeActorDto Actor { get; set; }
        public string ClientChangeId { get; set; }
        public IPatchExecutor<PatchDocumentChangeCommand> PatchExecutor { get; private set; }
    }
}