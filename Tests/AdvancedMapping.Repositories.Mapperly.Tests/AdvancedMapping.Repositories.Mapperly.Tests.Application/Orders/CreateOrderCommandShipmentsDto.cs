using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class CreateOrderCommandShipmentsDto
    {
        public CreateOrderCommandShipmentsDto()
        {
            Dispatch = null!;
            Manifest = null!;
            Provider = null!;
        }

        public CreateOrderCommandDispatchDto Dispatch { get; set; }
        public CreateOrderCommandManifestDto Manifest { get; set; }
        public string Provider { get; set; }
        public Guid? ContainerId { get; set; }

        public static CreateOrderCommandShipmentsDto Create(
            CreateOrderCommandDispatchDto dispatch,
            CreateOrderCommandManifestDto manifest, string provider, Guid? containerId)
        {
            return new CreateOrderCommandShipmentsDto
            {
                Dispatch = dispatch,
                Manifest = manifest,
                Provider = provider,
                ContainerId = containerId
            };
        }
    }
}