using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
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
            foreach (var skill in application.FindTemplateInstances<IMarkdownFileBuilderTemplate>(TemplateRoles.AI.Context.SkillsHandler))
            {
                skill.MarkdownFile.OnBuild(AddAutoMapperGuidanceSection);
            }
        }

        private static void AddAutoMapperGuidanceSection(IMarkdownFile file)
        {
            file.BeforeSection("Output expectations", "Mapperly guidance", section =>
            {
                section.WithListItems("""
            - Any read/query method, including query handlers and application services, that returns Application-layer DTOs (`*Dto`) derived from Domain entities **MUST** use Mapperly.
                - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths.
            - **Mapperly gate (absolute):** If you write `new XxxDto`, `.Select(x => new XxxDto...)`,
              or call any `*Mapper.Map*`/`*To*Dto*` method for an entity-derived DTO, you are **blocked**
              from writing the handler/service body until one of these two branches is complete:
                - **Branch A (verify):** locate a `[Mapper]` partial class with the required mapping
                  method, e.g. `CustomerToCustomerDto(Customer customer)`, and cite file path + excerpt.
                - **Branch B (create):** if no such mapper exists, that absence is itself the trigger to
                  create one now — it is never a reason to fall back to inline/manual mapping "for now"
                  or "as a quick fix". Create the mapper(s), including any nested mappers and collection
                  overloads (e.g. `CustomerToCustomerDtoList(...)`), before writing anything else.
                - **No assumptions allowed** (an existing mapper class for a *different* entity is not verification).
                - Before writing the handler/service, state explicitly which branch was taken:
                  `Mapper verified: <path>` or `Mapper created: <path>`. If neither statement is made,
                  do not write the handler/service.
            - **Registration assumption (do not block on DI):**
                - Assume mappers are registered as singletons per the project's existing DI style
                  (e.g. `services.AddSingleton<CustomerDtoMapper>();`).
                - Do not delay mapper creation because DI registration isn't currently visible.
                - Only add registration if you are the one creating a brand-new mapper class; otherwise leave DI alone.
            - Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation
              and Mapperly is not reasonable.
                - This must include an inline code comment explaining why Mapperly is not reasonable.
                - "Mapping doesn't exist yet" is not a valid exception.
            - If you can't find any existing mappings, create them in the same project as the services under:
                - `./Mappings/<FeatureOrAggregate>/<Entity>DtoMapper.cs`
                  
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
