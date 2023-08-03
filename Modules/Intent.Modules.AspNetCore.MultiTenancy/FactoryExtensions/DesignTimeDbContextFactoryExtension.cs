using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DesignTimeDbContextFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.AspNetCore.MultiTenancy.DesignTimeDbContextFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetMultitenancySettings().DataIsolation().IsSharedDatabase())
            {
                return;
            }
            
            var designTimeDbContextFactoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.DesignTimeDbContextFactory"));
            foreach (var template in designTimeDbContextFactoryTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("CreateDbContext");
                    var returnStatement = (CSharpInvocationStatement)method.Statements.LastOrDefault(p => p.HasMetadata("return-statement"));
                    returnStatement?.AddArgument("null", arg => arg.AddMetadata("tenant-info", true));
                });
            }
        }
    }
}