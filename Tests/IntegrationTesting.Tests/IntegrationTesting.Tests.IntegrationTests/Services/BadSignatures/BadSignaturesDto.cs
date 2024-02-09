using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures
{
    public class BadSignaturesDto
    {
        public BadSignaturesDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static BadSignaturesDto Create(Guid id, string name)
        {
            return new BadSignaturesDto
            {
                Id = id,
                Name = name
            };
        }
    }
}