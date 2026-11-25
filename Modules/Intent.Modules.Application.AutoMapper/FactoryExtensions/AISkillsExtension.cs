using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
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
            file.WithSection("Mapping (AutoMapper)", section =>
            {
                section.WithListItem("Return entity-shaped `*Dto` types through AutoMapper (`Map` or `ProjectTo`), not `new XxxDto { … }`. Before you use one, find its `CreateMap<TEntity, TDto>()` in a `Profile`. If there is none, add the profile, nested mappings included, as `Mappings/<Aggregate>/<Entity>DtoProfile.cs` in the Application project. Profiles are found by assembly scanning, so leave the AutoMapper registration alone.");
                section.WithListItem("Build a DTO by hand only when it is not entity-shaped, an aggregation for example, and say why in a comment.");
            });
        }
    }
}
