using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter1
{
    public class NullableRouteParameter1Command : IRequest, ICommand
    {
        public NullableRouteParameter1Command(string? nullableString,
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