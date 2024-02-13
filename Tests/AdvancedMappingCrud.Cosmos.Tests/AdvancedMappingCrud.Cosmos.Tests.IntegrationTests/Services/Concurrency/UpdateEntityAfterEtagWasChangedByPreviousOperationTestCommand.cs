using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Concurrency
{
    public class UpdateEntityAfterEtagWasChangedByPreviousOperationTestCommand
    {
        public static UpdateEntityAfterEtagWasChangedByPreviousOperationTestCommand Create()
        {
            return new UpdateEntityAfterEtagWasChangedByPreviousOperationTestCommand
            {
            };
        }
    }
}