using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasDateOnlyField.HasDateOnlyFields
{
    public class GetHasDateOnlyFieldsQuery
    {
        public static GetHasDateOnlyFieldsQuery Create()
        {
            return new GetHasDateOnlyFieldsQuery
            {
            };
        }
    }
}