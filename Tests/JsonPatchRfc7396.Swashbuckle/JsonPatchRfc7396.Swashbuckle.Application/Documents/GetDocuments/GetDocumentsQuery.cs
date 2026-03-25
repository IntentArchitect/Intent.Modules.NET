using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents.GetDocuments
{
    public class GetDocumentsQuery : IRequest<List<DocumentDto>>, IQuery
    {
        public GetDocumentsQuery()
        {
        }
    }
}