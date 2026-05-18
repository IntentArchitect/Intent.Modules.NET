using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class CreateOrderCommandDispatchDto
    {
        public CreateOrderCommandDispatchDto()
        {
            OriginLocation = null!;
            Document = null!;
        }

        public string OriginLocation { get; set; }
        public CreateOrderCommandDispatchDocumentDto Document { get; set; }

        public static CreateOrderCommandDispatchDto Create(
            string originLocation,
            CreateOrderCommandDispatchDocumentDto document)
        {
            return new CreateOrderCommandDispatchDto
            {
                OriginLocation = originLocation,
                Document = document
            };
        }
    }
}