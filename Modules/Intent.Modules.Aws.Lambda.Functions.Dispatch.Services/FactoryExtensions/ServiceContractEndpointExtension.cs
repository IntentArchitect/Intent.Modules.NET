using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.Services.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceContractEndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.Lambda.Functions.Dispatch.Services.ServiceContractEndpointExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            
        }
    }
}