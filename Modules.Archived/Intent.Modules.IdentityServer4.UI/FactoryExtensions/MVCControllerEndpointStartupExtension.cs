using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
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
    public class MVCControllerEndpointStartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IdentityServer4.UI.MVCControllerEndpointStartupExtension";

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
            var template = application.FindTemplateInstance<StartupTemplate>(StartupTemplate.TemplateId);
            template?.CSharpFile.AfterBuild(file =>
            {
                template.StartupFile.AddUseEndpointsStatement(c => new CSharpInvocationStatement($"{c.Endpoints}.MapControllerRoute")
                    .AddArgument(@"name: ""default""")
                    .AddArgument(@"pattern: ""{controller=Home}/{action=Index}/{id?}""")
                    .WithArgumentsOnNewLines());

                template.StartupFile.ConfigureEndpoints((stmts, context) =>
                {
                    stmts.FindStatement(p => p.HasMetadata("configure-endpoints-controllers-generic"))
                        ?.Remove();
                });
            }, -30);
        }
    }
}