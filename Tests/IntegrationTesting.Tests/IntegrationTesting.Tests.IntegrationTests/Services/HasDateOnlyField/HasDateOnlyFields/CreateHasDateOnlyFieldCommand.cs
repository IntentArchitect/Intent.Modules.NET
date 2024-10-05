using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields
{
    public class CreateHasDateOnlyFieldCommand
    {
        public DateOnly MyDate { get; set; }

        public static CreateHasDateOnlyFieldCommand Create(DateOnly myDate)
        {
            return new CreateHasDateOnlyFieldCommand
            {
                MyDate = myDate
            };
        }
    }
}