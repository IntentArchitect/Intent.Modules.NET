using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.GetDerivedTypes
{
    public class GetDerivedTypesQuery : IRequest<List<DerivedTypeDto>>, IQuery
    {
        public GetDerivedTypesQuery()
        {
        }
    }
}