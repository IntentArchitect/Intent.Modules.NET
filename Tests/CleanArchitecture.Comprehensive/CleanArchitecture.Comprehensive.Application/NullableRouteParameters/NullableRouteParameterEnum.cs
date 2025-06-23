using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters
{
    public enum NullableRouteParameterEnum
    {
        Option1 = 1,
        Option2 = 2
    }
}