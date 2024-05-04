using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAlternates
{
    public class UpdateEntityAlternateCommand
    {
        public UpdateEntityAlternateCommand()
        {
            Message = null!;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }

        public static UpdateEntityAlternateCommand Create(Guid id, string message)
        {
            return new UpdateEntityAlternateCommand
            {
                Id = id,
                Message = message
            };
        }
    }
}