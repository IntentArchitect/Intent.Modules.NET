using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAlternates
{
    public class GetEntityAlternatesQuery
    {
        public static GetEntityAlternatesQuery Create()
        {
            return new GetEntityAlternatesQuery
            {
            };
        }
    }
}