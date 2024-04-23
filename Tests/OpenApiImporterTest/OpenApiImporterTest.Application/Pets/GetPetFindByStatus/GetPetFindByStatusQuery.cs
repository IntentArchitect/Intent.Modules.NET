using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetPetFindByStatus
{
    public class GetPetFindByStatusQuery : IRequest<List<Pet>>, IQuery
    {
        public GetPetFindByStatusQuery(string status)
        {
            Status = status;
        }

        public string Status { get; set; }
    }
}