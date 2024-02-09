using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class GetUserByIdQuery
    {
        public Guid Id { get; set; }

        public static GetUserByIdQuery Create(Guid id)
        {
            return new GetUserByIdQuery
            {
                Id = id
            };
        }
    }
}