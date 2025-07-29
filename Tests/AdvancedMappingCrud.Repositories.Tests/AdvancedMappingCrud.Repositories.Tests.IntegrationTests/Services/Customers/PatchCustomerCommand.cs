using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class PatchCustomerCommand
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public bool? IsActive { get; set; }
        public bool? Newsletter { get; set; }
        public bool? Specials { get; set; }

        public static PatchCustomerCommand Create(
            Guid id,
            string? name,
            string? surname,
            bool? isActive,
            bool? newsletter,
            bool? specials)
        {
            return new PatchCustomerCommand
            {
                Id = id,
                Name = name,
                Surname = surname,
                IsActive = isActive,
                Newsletter = newsletter,
                Specials = specials
            };
        }
    }
}