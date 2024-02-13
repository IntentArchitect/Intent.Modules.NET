using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Concurrency
{
    public class UpdateEntityAfterEtagWasChangedByPreviousOperationTest
    {
        public static UpdateEntityAfterEtagWasChangedByPreviousOperationTest Create()
        {
            return new UpdateEntityAfterEtagWasChangedByPreviousOperationTest
            {
            };
        }
    }
}