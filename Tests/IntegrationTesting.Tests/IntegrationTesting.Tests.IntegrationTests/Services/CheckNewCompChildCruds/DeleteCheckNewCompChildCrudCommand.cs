using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class DeleteCheckNewCompChildCrudCommand
    {
        public Guid Id { get; set; }

        public static DeleteCheckNewCompChildCrudCommand Create(Guid id)
        {
            return new DeleteCheckNewCompChildCrudCommand
            {
                Id = id
            };
        }
    }
}