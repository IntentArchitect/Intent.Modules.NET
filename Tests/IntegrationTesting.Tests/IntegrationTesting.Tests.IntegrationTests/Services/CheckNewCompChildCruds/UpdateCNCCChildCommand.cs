using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class UpdateCNCCChildCommand
    {
        public UpdateCNCCChildCommand()
        {
            Description = null!;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }

        public static UpdateCNCCChildCommand Create(Guid checkNewCompChildCrudId, string description, Guid id)
        {
            return new UpdateCNCCChildCommand
            {
                CheckNewCompChildCrudId = checkNewCompChildCrudId,
                Description = description,
                Id = id
            };
        }
    }
}