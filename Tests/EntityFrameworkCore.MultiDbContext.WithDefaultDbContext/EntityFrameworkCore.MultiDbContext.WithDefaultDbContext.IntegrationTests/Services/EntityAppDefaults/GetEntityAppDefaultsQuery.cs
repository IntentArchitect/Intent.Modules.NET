using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAppDefaults
{
    public class GetEntityAppDefaultsQuery
    {
        public static GetEntityAppDefaultsQuery Create()
        {
            return new GetEntityAppDefaultsQuery
            {
            };
        }
    }
}