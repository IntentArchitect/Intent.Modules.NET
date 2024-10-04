using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.EnumContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Validation
{
    public enum EnumDescriptions
    {
        [Description("My First Option")]
        Option1,
        [Description("My Second Option")]
        Option2 = 2
    }
}