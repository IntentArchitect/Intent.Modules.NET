using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Rejected
    }
}