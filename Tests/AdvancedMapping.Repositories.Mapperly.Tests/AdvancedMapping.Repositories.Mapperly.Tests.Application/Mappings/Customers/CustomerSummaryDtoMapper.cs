using AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers
{
    [Mapper]
    public partial class CustomerSummaryDtoMapper
    {
        [MapperIgnoreSource(nameof(Customer.IsVip))]
        [MapperIgnoreSource(nameof(Customer.BirthDate))]
        [MapperIgnoreSource(nameof(Customer.MetadataJson))]
        public partial CustomerSummaryDto CustomerToCustomerSummaryDto(Customer customer);

        public partial List<CustomerSummaryDto> CustomerToCustomerSummaryDtoList(IEnumerable<Customer> customers);
    }
}