using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter.HasDtoParameter
{
    public class HasDtoParameterQuery : IRequest<int>, IQuery
    {
        public HasDtoParameterQuery(QueryDtoParameterCriteria arg)
        {
            Arg = arg;
        }

        public QueryDtoParameterCriteria Arg { get; set; }
    }
}