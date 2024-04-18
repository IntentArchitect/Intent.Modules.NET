using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.EnumRoute
{
    public enum Company
    {
        CompanyA = 1,
        CompanyB = 2
    }
}