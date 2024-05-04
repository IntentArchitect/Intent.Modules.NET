using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAlternates
{
    public class CreateEntityAlternateCommand
    {
        public CreateEntityAlternateCommand()
        {
            Message = null!;
        }

        public string Message { get; set; }

        public static CreateEntityAlternateCommand Create(string message)
        {
            return new CreateEntityAlternateCommand
            {
                Message = message
            };
        }
    }
}