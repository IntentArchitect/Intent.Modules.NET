using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures
{
    public class CreateBadSignaturesCommand
    {
        public CreateBadSignaturesCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateBadSignaturesCommand Create(string name)
        {
            return new CreateBadSignaturesCommand
            {
                Name = name
            };
        }
    }
}