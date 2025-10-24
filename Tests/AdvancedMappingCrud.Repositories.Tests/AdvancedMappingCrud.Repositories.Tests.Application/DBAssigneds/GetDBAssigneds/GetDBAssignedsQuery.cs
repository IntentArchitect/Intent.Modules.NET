using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DBAssigneds.GetDBAssigneds
{
    public class GetDBAssignedsQuery : IRequest<List<DBAssignedDto>>, IQuery
    {
        public GetDBAssignedsQuery()
        {
        }
    }
}