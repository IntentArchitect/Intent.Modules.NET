using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class CustomerSummaryDto
    {
        public CustomerSummaryDto()
        {
            Name = null!;
            Email = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public static CustomerSummaryDto Create(Guid id, string name, string email)
        {
            return new CustomerSummaryDto
            {
                Id = id,
                Name = name,
                Email = email
            };
        }
    }
}