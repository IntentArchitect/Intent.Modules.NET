using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents.GetDocumentById
{
    public class GetDocumentByIdQuery : IRequest<DocumentDto>, IQuery
    {
        public GetDocumentByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}