using AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers
{
    [Mapper]
    public partial class CustomerLeanDtoMapper
    {
        [MapperIgnoreSource(nameof(Customer.Email))]
        [MapperIgnoreSource(nameof(Customer.IsVip))]
        [MapperIgnoreSource(nameof(Customer.BirthDate))]
        [MapperIgnoreSource(nameof(Customer.MetadataJson))]
        public partial CustomerLeanDto CustomerToCustomerLeanDto(Customer customer);

        public partial List<CustomerLeanDto> CustomerToCustomerLeanDtoList(IEnumerable<Customer> customers);
    }
}