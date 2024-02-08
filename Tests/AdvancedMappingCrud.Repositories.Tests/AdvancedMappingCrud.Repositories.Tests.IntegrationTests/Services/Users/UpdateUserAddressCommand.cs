using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class UpdateUserAddressCommand
    {
        public UpdateUserAddressCommand()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
        }

        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }

        public static UpdateUserAddressCommand Create(Guid userId, Guid id, string line1, string line2, string city, string postal)
        {
            return new UpdateUserAddressCommand
            {
                UserId = userId,
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }
    }
}