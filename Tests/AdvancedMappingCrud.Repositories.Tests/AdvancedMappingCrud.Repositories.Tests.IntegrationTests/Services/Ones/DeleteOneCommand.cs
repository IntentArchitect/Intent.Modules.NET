using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class DeleteOneCommand
    {
        public Guid Id { get; set; }

        public static DeleteOneCommand Create(Guid id)
        {
            return new DeleteOneCommand
            {
                Id = id
            };
        }
    }
}