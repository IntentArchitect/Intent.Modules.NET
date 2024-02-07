using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures
{
    public class GetBadSignaturesByIdQuery
    {
        public Guid Id { get; set; }

        public static GetBadSignaturesByIdQuery Create(Guid id)
        {
            return new GetBadSignaturesByIdQuery
            {
                Id = id
            };
        }
    }
}