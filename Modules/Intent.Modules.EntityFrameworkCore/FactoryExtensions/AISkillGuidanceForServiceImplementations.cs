using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AISkillGuidanceForServiceImplementations : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.AISkillGuidanceForServiceImplementations";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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

            skill?.MarkdownFile.OnBuild((file) => AddEFGuidanceSection(application, file));
        }

        private static void AddEFGuidanceSection(IApplication application, IMarkdownFile file)
        {
            var lazyLoading = application.GetSettings().GetDatabaseSettings().LazyLoadingWithProxies();

            file.BeforeSection("Output expectations", "EF Related Data Loading guidance", section =>
            {
                section.WithListItems($"""
            - NEVER use `Include` or `ThenInclude` in the Application Layer, these are only available in the Infrastructure layer.
            - Lazy loading with proxies is {(lazyLoading ? "enabled" : "disabled")}. 
            - Entities are configured using the `Owns` apis, so compsitional children will be automatically loaded with their parents.
            """);
                if (lazyLoading)
                {
                    section.WithListItem("You can rely on navigation properties being automatically loaded when accessed.");
                    section.WithListItem("(CRITICAL) If your implementation will cause a lot of Lazy loading consider other alternatives, like moving the data loading into the repository layer.");
                }
                else
                {
                    section.WithListItem("You must explicitly load related data in the infrastructure layer using repository methods");
                }
            });
        }
    }
}