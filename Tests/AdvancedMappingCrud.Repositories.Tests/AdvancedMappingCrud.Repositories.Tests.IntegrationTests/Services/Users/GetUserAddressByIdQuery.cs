using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class GetUserAddressByIdQuery
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }

        public static GetUserAddressByIdQuery Create(Guid userId, Guid id)
        {
            return new GetUserAddressByIdQuery
            {
                UserId = userId,
                Id = id
            };
        }
    }
}