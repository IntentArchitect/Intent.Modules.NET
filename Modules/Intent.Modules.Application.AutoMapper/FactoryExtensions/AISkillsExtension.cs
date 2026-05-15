using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AISkillsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.AutoMapper.AISkillsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 1; // Made this 1 so this runs after the UOWs (just for backs compatibility)
       
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterAutoMapperGuidance(
                application,
                "Intent.Application.MediatR.CommandHandlerSkillTemplate");

            RegisterAutoMapperGuidance(
                application,
                "Intent.Application.MediatR.QueryHandlerSkillTemplate");

            RegisterAutoMapperGuidance(
                application,
                "Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate");
        }

        private static void RegisterAutoMapperGuidance(
            IApplication application,
            string templateId)
        {
            var skill = application.FindTemplateInstance<IMarkdownFileBuilderTemplate>(templateId);

            skill?.MarkdownFile.OnBuild(AddAutoMapperGuidanceSection);
        }

        private static void AddAutoMapperGuidanceSection(IMarkdownFile file)
        {
            file.BeforeSection("Output expectations", "AutoMapper guidance", section =>
            {
                section.WithListItems("""
            - Any read/query method, including MediatR query handlers and application services, that returns Application-layer DTOs (`*Dto`) derived from Domain entities **MUST** use AutoMapper.
                - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths.
            - **AutoMapper gate (absolute):** If you use any `ProjectTo*`, `Find*ProjectTo*`, `FindAllProjectTo*`, or `*ProjectToAsync*` method anywhere in the call chain, you **MUST**:
                - **verify mapping exists** by locating `CreateMap<TDomain, TDto>()` in a `Profile` **and cite file path + excerpt**, **OR**
                - if verification fails, **immediately create** the required AutoMapper `Profile`(s) (including **all required nested mappings**).
                - **No assumptions allowed** (a generic projection method or other feature usage is not verification).
            - **Registration assumption (do not block on DI):**
                - Assume AutoMapper is registered via assembly scanning, e.g.:services.AddAutoMapper(Assembly.GetExecutingAssembly());
                - Therefore, **do not delay profile creation** because DI registration details are not currently visible.
                - Do not modify DI registration as part of this guidance unless the user explicitly asks.
            - Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and AutoMapper is not reasonable.
                - This must include an inline code comment explaining why AutoMapper is not reasonable.
                - “Mapping doesn’t exist yet” is not a valid exception.
            - If you can't find any existing mappings, create them in the same project as the services under:
                - `./Mappings/<FeatureOrAggregate>/<Entity>DtoProfile.cs`
                - Example: `MyApp.Application/Mappings/Invoices/InvoiceDtoProfile.cs`            
            """);

                section.WithCodeBlock("""
            
            public class CustomerDtoProfile : Profile
            {
                public CustomerDtoProfile()
                {
                    CreateMap<Customer, CustomerDto>();
                }
            }

            public static class CustomerDtoMappingExtensions
            {
                public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper) =>
                    mapper.Map<CustomerDto>(projectFrom);

                public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper) =>
                    projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
            }
            """, "csharp", "Example:");
            });                
        }
    }
}