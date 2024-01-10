using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RazorPageControllerStartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IdentityServer4.UI.RazorPageControllerStartupExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Distribution.WebApi.Startup);
            template?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var configServicesStmts = @class.Methods
                    .First(x => x.Name == "ConfigureServices")
                    .Statements;

                configServicesStmts.First().InsertAbove("services.AddControllersWithViews();")
                    .InsertBelow("services.AddRazorPages();");

                configServicesStmts
                    .FirstOrDefault(x => x.HasMetadata("configure-services-controllers-generic"))
                    ?.Remove();
            }, -30);
        }
    }
}