using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocById
{
    public class GetExternalDocByIdQuery : IRequest<ExternalDocDto>, IQuery
    {
        public GetExternalDocByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}