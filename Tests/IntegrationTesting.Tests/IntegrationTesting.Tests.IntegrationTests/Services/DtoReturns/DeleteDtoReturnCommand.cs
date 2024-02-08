using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns
{
    public class DeleteDtoReturnCommand
    {
        public Guid Id { get; set; }

        public static DeleteDtoReturnCommand Create(Guid id)
        {
            return new DeleteDtoReturnCommand
            {
                Id = id
            };
        }
    }
}