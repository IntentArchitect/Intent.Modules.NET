using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.Validation
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