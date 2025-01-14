using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class CreateCNCCChildCommand
    {
        public CreateCNCCChildCommand()
        {
            Description = null!;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public string Description { get; set; }

        public static CreateCNCCChildCommand Create(Guid checkNewCompChildCrudId, string description)
        {
            return new CreateCNCCChildCommand
            {
                CheckNewCompChildCrudId = checkNewCompChildCrudId,
                Description = description
            };
        }
    }
}