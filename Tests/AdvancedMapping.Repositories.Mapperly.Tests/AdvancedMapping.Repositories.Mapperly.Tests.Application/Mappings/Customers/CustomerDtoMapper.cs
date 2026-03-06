using AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers
{
    [Mapper]
    public partial class CustomerDtoMapper
    {

        [MapPropertyFromSource(nameof(CustomerDto.PreferencesNewsletter), Use = nameof(MapPreferencesNewsletter))]
        [MapPropertyFromSource(nameof(CustomerDto.PreferencesSpecials), Use = nameof(MapPreferencesSpecials))]
        public partial CustomerDto CustomerToCustomerDto(Customer customer);

        public partial List<CustomerDto> CustomerToCustomerDtoList(List<Customer> customers);

        private bool? MapPreferencesNewsletter(Customer source) => (bool?)source.Preferences?.Newsletter;

        private bool? MapPreferencesSpecials(Customer source) => (bool?)source.Preferences?.Specials;
    }
}