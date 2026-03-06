using AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers
{
    [Mapper]
    public partial class CustomerPreferencesDtoMapper
    {
        public partial CustomerPreferencesDto PreferencesToCustomerPreferencesDto(Preferences preferences);

        public partial List<CustomerPreferencesDto> PreferencesToCustomerPreferencesDtoList(List<Preferences> preferences);
    }
}