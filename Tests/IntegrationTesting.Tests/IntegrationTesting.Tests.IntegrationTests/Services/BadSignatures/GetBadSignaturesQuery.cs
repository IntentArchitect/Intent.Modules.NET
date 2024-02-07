using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures
{
    public class GetBadSignaturesQuery
    {
        public GetBadSignaturesQuery()
        {
            Filter = null!;
        }

        public string Filter { get; set; }

        public static GetBadSignaturesQuery Create(string filter)
        {
            return new GetBadSignaturesQuery
            {
                Filter = filter
            };
        }
    }
}