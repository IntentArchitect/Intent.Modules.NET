using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.NoReturns
{
    public class DeleteNoReturnCommand
    {
        public Guid Id { get; set; }

        public static DeleteNoReturnCommand Create(Guid id)
        {
            return new DeleteNoReturnCommand
            {
                Id = id
            };
        }
    }
}