using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AISkillsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Mapperly.AISkillsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 1; // Made this 1 so this runs after the UOWs (just for backs compatibility)
       
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterMapperlyGuidance(
                application,
                "Intent.Application.MediatR.CommandHandlerSkillTemplate");

            RegisterMapperlyGuidance(
                application,
                "Intent.Application.MediatR.QueryHandlerSkillTemplate");

            RegisterMapperlyGuidance(
                application,
                "Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate");
        }

        private static void RegisterMapperlyGuidance(
            IApplication application,
            string templateId)
        {
            var skill = application.FindTemplateInstance<IMarkdownFileBuilderTemplate>(templateId);

            skill?.MarkdownFile.OnBuild(AddAutoMapperGuidanceSection);
        }

        private static void AddAutoMapperGuidanceSection(IMarkdownFile file)
        {
            file.BeforeSection("Output expectations", "Mapperly guidance", section =>
            {
                section.WithListItems("""
            - Any read/query method, including MediatR query handlers and application services, that returns Application-layer DTOs (`*Dto`) derived from Domain entities **MUST** use Mapperly.
                - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths..
            - **Mapperly gate (absolute):** If a handler/service returns entity-shaped DTOs or uses any mapper call, you **MUST**:
                - verify a Mapperly mapper exists by locating a `[Mapper]` partial mapper class with the required mapping method, e.g. `CustomerToCustomerDto(Customer customer)`, **and cite file path + excerpt**, **OR**
                - if verification fails, **immediately create** the required Mapperly mapper(s), including all required nested mappers.
                - verify collection mappings when returning lists, e.g. `CustomerToCustomerDtoList(IEnumerable<Customer> customers)`.
                - verify nested mapper dependencies use `[UseMapper]` and constructor injection where needed.
            - **Registration gate:**
                - If a mapper is injected into a handler/service, verify it is registered in Application DI.
                - Follow the existing registration style. Mapperly sample projects register mappers as singletons, e.g. `services.AddSingleton<CustomerDtoMapper>();`.
                - If registration is missing, add the minimal mapper registration, including nested mapper registrations.
            - Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and Mapperly is not reasonable.
                - This must include an inline code comment explaining why Mapperly is not reasonable.
                - “Mapping doesn’t exist yet” is not a valid exception.
            - If you can't find any existing mappings, create them in the same project as the services under:
                - `./Mappings/<FeatureOrAggregate>/<Entity>DtoMapper.cs`
                - Example: `MyApp.Application/Mappings/Invoices/InvoiceDtoMapper.cs`        
            """);

                section.WithCodeBlock("""
                [Mapper]
                public partial class OrderDtoMapper
                {
                    [UseMapper]
                    private readonly OrderLineDtoMapper _orderLineDtoMapper;

                    public OrderDtoMapper(OrderLineDtoMapper orderLineDtoMapper)
                    {
                        _orderLineDtoMapper = orderLineDtoMapper;
                    }

                    [MapProperty(nameof(Order.Lines), nameof(OrderDto.OrderLines))]
                    [MapPropertyFromSource(nameof(OrderDto.IsActive), Use = nameof(MapIsActive))]
                    public partial OrderDto OrderToOrderDto(Order order);

                    public partial List<OrderDto> OrderToOrderDtoList(IEnumerable<Order> orders);

                    private bool MapIsActive(Order source) => source.IsActive();
            }
            """, "csharp", "Example:");
            });                
        }
    }
}