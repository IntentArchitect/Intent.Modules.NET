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
            - Any read/query method, including application services, that returns Application-layer DTOs (*Dto) derived from Domain entities must use AutoMapper.
              - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths.
            - If the required mapping does not exist, create it:
              - Add an AutoMapper Profile.
              - Include mapping extension methods in the same file, matching existing conventions.
            - Before using repository `ProjectTo` operations, verify that the required AutoMapper mappings exist.
            - Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and AutoMapper is not reasonable.
              - This must include an inline code comment explaining why AutoMapper is not reasonable.
              - “Mapping doesn’t exist yet” is not a valid exception.
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