using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields
{
    public class GetHasDateOnlyFieldByIdQuery
    {
        public Guid Id { get; set; }

        public static GetHasDateOnlyFieldByIdQuery Create(Guid id)
        {
            return new GetHasDateOnlyFieldByIdQuery
            {
                Id = id
            };
        }
    }
}