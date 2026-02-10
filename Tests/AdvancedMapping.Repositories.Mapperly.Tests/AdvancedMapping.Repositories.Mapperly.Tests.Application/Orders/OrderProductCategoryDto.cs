using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderProductCategoryDto
    {
        public OrderProductCategoryDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static OrderProductCategoryDto Create(Guid id, string name)
        {
            return new OrderProductCategoryDto
            {
                Id = id,
                Name = name
            };
        }
    }
}