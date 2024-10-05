using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields
{
    public class DeleteHasDateOnlyFieldCommand
    {
        public Guid Id { get; set; }

        public static DeleteHasDateOnlyFieldCommand Create(Guid id)
        {
            return new DeleteHasDateOnlyFieldCommand
            {
                Id = id
            };
        }
    }
}