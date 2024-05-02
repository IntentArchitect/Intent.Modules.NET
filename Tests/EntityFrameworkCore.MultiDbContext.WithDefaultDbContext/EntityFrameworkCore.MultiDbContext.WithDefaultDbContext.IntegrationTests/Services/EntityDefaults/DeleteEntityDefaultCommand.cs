using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityDefaults
{
    public class DeleteEntityDefaultCommand
    {
        public Guid Id { get; set; }

        public static DeleteEntityDefaultCommand Create(Guid id)
        {
            return new DeleteEntityDefaultCommand
            {
                Id = id
            };
        }
    }
}