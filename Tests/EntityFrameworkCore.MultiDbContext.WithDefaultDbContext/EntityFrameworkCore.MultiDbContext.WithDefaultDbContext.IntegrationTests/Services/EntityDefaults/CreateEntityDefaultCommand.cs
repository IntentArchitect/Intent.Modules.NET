using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityDefaults
{
    public class CreateEntityDefaultCommand
    {
        public CreateEntityDefaultCommand()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static CreateEntityDefaultCommand Create(string message)
        {
            return new CreateEntityDefaultCommand
            {
                Message = message
            };
        }
    }
}