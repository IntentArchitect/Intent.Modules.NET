using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Enums
{
    public enum Company
    {
        CompanyA = 1,
        CompanyB = 2
    }
}