using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields
{
    public class HasDateOnlyFieldDto
    {
        public Guid Id { get; set; }
        public DateOnly MyDate { get; set; }

        public static HasDateOnlyFieldDto Create(Guid id, DateOnly myDate)
        {
            return new HasDateOnlyFieldDto
            {
                Id = id,
                MyDate = myDate
            };
        }
    }
}