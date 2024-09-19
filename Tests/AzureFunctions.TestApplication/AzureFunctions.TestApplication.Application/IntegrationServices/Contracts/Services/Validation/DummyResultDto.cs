using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Validation
{
    public class DummyResultDto
    {
        public static DummyResultDto Create()
        {
            return new DummyResultDto
            {
            };
        }
    }
}