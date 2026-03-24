using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Common.Exceptions;
using JsonPatchRfc7396.Swashbuckle.Domain.Repositories.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.DeleteDocument
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand>
    {
        private readonly IDocumentRepository _documentRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDocumentCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (document is null)
            {
                throw new NotFoundException($"Could not find Document '{request.Id}'");
            }


            _documentRepository.Remove(document);
        }
    }
}