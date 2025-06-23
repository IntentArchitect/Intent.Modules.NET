using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter2
{
    public class NullableRouteParameter2Query : IRequest<int>, IQuery
    {
        public NullableRouteParameter2Query(string? nullableString,
            int? nullableInt,
            NullableRouteParameterEnum? nullableEnum)
        {
            NullableString = nullableString;
            NullableInt = nullableInt;
            NullableEnum = nullableEnum;
        }

        public string? NullableString { get; set; }
        public int? NullableInt { get; set; }
        public NullableRouteParameterEnum? NullableEnum { get; set; }
    }
}