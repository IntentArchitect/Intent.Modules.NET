using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetByTags
{
    [Authorize]
    public class GetByTagsQuery : IRequest<List<Pet>>, IQuery
    {
        public GetByTagsQuery(List<string> tags)
        {
            Tags = tags;
        }

        public List<string> Tags { get; set; }
    }
}