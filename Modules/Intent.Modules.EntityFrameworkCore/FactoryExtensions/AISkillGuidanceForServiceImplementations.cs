using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
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

        private const string PersistenceSectionTitle = "Persistence (EF Core)";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            foreach (var skill in application.FindTemplateInstances<IMarkdownFileBuilderTemplate>(TemplateRoles.AI.Context.SkillsHandler))
            {
                skill.MarkdownFile.OnBuild(file => AddEFGuidanceSection(application, file));
            }
        }

        private static void AddEFGuidanceSection(IApplication application, IMarkdownFile file)
        {
            var lazyLoading = application.GetSettings().GetDatabaseSettings().LazyLoadingWithProxies();

            // "Persistence (EF Core)" is shared with Intent.EntityFrameworkCore.Repositories, which adds its
            // unit-of-work and Update rules to the same section after this one.
            file.WithSection(PersistenceSectionTitle, section =>
            {
                section.WithListItem(lazyLoading
                    ? "`Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, and lazy loading with proxies is enabled, so navigation properties load when accessed. If that would mean many lazy loads, move the loading into a repository method."
                    : "`Include` and `ThenInclude` are Infrastructure-only. Owned children load with their parent, but lazy loading is off, so explicitly load any other related data in a repository method.");
            });
        }
    }
}
