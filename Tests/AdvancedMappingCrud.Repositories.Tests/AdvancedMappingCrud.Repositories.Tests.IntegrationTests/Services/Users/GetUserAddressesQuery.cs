using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class GetUserAddressesQuery
    {
        public Guid UserId { get; set; }

        public static GetUserAddressesQuery Create(Guid userId)
        {
            return new GetUserAddressesQuery
            {
                UserId = userId
            };
        }
    }
}