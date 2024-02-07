using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class DeleteUserAddressCommand
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }

        public static DeleteUserAddressCommand Create(Guid userId, Guid id)
        {
            return new DeleteUserAddressCommand
            {
                UserId = userId,
                Id = id
            };
        }
    }
}