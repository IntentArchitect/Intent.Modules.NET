using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields
{
    public class UpdateHasDateOnlyFieldCommand
    {
        public DateOnly MyDate { get; set; }
        public Guid Id { get; set; }

        public static UpdateHasDateOnlyFieldCommand Create(DateOnly myDate, Guid id)
        {
            return new UpdateHasDateOnlyFieldCommand
            {
                MyDate = myDate,
                Id = id
            };
        }
    }
}