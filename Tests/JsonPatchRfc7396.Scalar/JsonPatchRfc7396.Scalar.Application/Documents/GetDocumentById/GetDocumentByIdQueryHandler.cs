using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Common.Exceptions;
using JsonPatchRfc7396.Scalar.Domain.Repositories.CollaborativeEditing;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.GetDocumentById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDocumentByIdQueryHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (document is null)
            {
                throw new NotFoundException($"Could not find Document '{request.Id}'");
            }
            return document.MapToDocumentDto(_mapper);
        }
    }
}