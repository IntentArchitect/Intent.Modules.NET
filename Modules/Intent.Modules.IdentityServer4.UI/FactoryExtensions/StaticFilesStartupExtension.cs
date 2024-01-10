using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.Plugins;
using Intent.Modules.IdentityServer4.UI.Templates.QuickStartContent;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StaticFilesStartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IdentityServer4.UI.StaticFilesStartupExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseStaticFiles")
                .WithPriority(-15));
        }
    }
}