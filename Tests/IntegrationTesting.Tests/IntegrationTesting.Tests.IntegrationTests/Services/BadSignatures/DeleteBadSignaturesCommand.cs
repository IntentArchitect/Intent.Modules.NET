using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures
{
    public class DeleteBadSignaturesCommand
    {
        public DeleteBadSignaturesCommand()
        {
            More = null!;
        }

        public Guid Id { get; set; }
        public string More { get; set; }

        public static DeleteBadSignaturesCommand Create(Guid id, string more)
        {
            return new DeleteBadSignaturesCommand
            {
                Id = id,
                More = more
            };
        }
    }
}