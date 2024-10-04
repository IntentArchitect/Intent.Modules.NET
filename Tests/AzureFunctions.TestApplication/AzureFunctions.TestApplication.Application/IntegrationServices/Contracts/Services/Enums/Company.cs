using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.EnumContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Enums
{
    public enum Company
    {
        CompanyA = 1,
        CompanyB = 2
    }
}