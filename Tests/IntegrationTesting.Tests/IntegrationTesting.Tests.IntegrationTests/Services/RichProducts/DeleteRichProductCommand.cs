using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.RichProducts
{
    public class DeleteRichProductCommand
    {
        public Guid Id { get; set; }

        public static DeleteRichProductCommand Create(Guid id)
        {
            return new DeleteRichProductCommand
            {
                Id = id
            };
        }
    }
}