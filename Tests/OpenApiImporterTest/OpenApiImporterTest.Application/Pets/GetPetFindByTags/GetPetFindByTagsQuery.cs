using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetPetFindByTags
{
    public class GetPetFindByTagsQuery : IRequest<List<Pet>>, IQuery
    {
        public GetPetFindByTagsQuery(List<string> tags)
        {
            Tags = tags;
        }

        public List<string> Tags { get; set; }
    }
}