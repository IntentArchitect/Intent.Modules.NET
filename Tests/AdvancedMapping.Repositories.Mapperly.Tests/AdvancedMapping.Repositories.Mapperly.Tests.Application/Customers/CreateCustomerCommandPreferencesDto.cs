using AdvancedMapping.Repositories.Mapperly.Tests.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class CreateCustomerCommandPreferencesDto
    {
        public CreateCustomerCommandPreferencesDto()
        {
        }

        public bool Newsletter { get; set; }
        public bool Specials { get; set; }
        public Theme Theme { get; set; }

        public static CreateCustomerCommandPreferencesDto Create(bool newsletter, bool specials, Theme theme)
        {
            return new CreateCustomerCommandPreferencesDto
            {
                Newsletter = newsletter,
                Specials = specials,
                Theme = theme
            };
        }
    }
}