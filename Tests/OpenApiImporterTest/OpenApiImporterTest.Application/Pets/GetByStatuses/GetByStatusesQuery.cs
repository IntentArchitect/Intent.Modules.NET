using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetByStatuses
{
    [Authorize]
    public class GetByStatusesQuery : IRequest<List<Pet>>, IQuery
    {
        public GetByStatusesQuery(string status)
        {
            Status = status;
        }

        public string Status { get; set; }
    }
}