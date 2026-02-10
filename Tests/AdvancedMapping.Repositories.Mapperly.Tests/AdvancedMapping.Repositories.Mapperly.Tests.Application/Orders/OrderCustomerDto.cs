using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderCustomerDto
    {
        public OrderCustomerDto()
        {
            Name = null!;
            Email = null!;
            Addresses = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsVip { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? MetadataJson { get; set; }
        public List<OrderAddressDto> Addresses { get; set; }

        public static OrderCustomerDto Create(
            Guid id,
            string name,
            string email,
            bool isVip,
            DateTime? birthDate,
            string? metadataJson,
            List<OrderAddressDto> addresses)
        {
            return new OrderCustomerDto
            {
                Id = id,
                Name = name,
                Email = email,
                IsVip = isVip,
                BirthDate = birthDate,
                MetadataJson = metadataJson,
                Addresses = addresses
            };
        }
    }
}