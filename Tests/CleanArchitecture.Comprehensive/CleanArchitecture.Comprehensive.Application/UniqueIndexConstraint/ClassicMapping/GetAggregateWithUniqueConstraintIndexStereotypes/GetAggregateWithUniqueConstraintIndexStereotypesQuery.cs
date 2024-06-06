using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.GetAggregateWithUniqueConstraintIndexStereotypes
{
    public class GetAggregateWithUniqueConstraintIndexStereotypesQuery : IRequest<List<AggregateWithUniqueConstraintIndexStereotypeDto>>, IQuery
    {
        public GetAggregateWithUniqueConstraintIndexStereotypesQuery()
        {
        }
    }
}