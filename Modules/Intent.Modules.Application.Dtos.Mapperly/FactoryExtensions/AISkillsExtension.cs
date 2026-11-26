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
        public override int Order => 2; // After the EF Core and EF repository guidance (Orders 0 and 1), so "Mapping" follows "Persistence"
       
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            foreach (var skill in application.FindTemplateInstances<IMarkdownFileBuilderTemplate>(TemplateRoles.AI.Context.SkillsHandler))
            {
                skill.MarkdownFile.OnBuild(AddAutoMapperGuidanceSection);
            }
        }

        private static void AddAutoMapperGuidanceSection(IMarkdownFile file)
        {
            file.WithSection("Mapping (Mapperly)", section =>
            {
                section.WithListItem("Return entity-shaped `*Dto` types through a Mapperly mapper, not `new XxxDto { … }`. Before you use one, find the `[Mapper]` partial class with its mapping method, for example `CustomerToCustomerDto(Customer customer)`; a mapper for a different entity does not count. If there is none, add it, nested mappers (`[UseMapper]`) and list overloads included, as `Mappings/<Aggregate>/<Entity>DtoMapper.cs` in the Application project, and register a new mapper the way the existing ones are, usually `services.AddSingleton<CustomerDtoMapper>()`.");
                section.WithListItem("Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment. A missing mapper is not a reason, not even for now: add it first.");
                section.WithCodeBlock("""
                    // Mappings/Customers/CustomerDtoMapper.cs
                    [Mapper]
                    public partial class CustomerDtoMapper
                    {
                        public partial CustomerDto CustomerToCustomerDto(Customer customer);
                        public partial List<CustomerDto> CustomerToCustomerDtoList(IEnumerable<Customer> customers);
                    }

                    // In the handler or service, with CustomerDtoMapper injected
                    return _mapper.CustomerToCustomerDto(customer);
                    """, "csharp", "Example");
            });
        }
    }
}
