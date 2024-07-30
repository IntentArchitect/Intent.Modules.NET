using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Players
{
    public enum PriceChangeType
    {
        None = 0,
        Any = 1,
        Higher = 2
    }
}