using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAppDefaults
{
    public class UpdateEntityAppDefaultCommand
    {
        public UpdateEntityAppDefaultCommand()
        {
            Message = null!;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }

        public static UpdateEntityAppDefaultCommand Create(Guid id, string message)
        {
            return new UpdateEntityAppDefaultCommand
            {
                Id = id,
                Message = message
            };
        }
    }
}