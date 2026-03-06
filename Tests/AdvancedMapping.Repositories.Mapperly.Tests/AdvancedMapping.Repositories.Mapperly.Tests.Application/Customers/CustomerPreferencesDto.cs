using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class CustomerPreferencesDto
    {
        public CustomerPreferencesDto()
        {
        }

        public Guid Id { get; set; }
        public bool Newsletter { get; set; }
        public bool Specials { get; set; }

        public static CustomerPreferencesDto Create(Guid id, bool newsletter, bool specials)
        {
            return new CustomerPreferencesDto
            {
                Id = id,
                Newsletter = newsletter,
                Specials = specials
            };
        }
    }
}