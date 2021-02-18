using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Plugins;
using Intent.Modules.IdentityServer4.SecureTokenServer.Contracts;
using Intent.Modules.IdentityServer4.SecureTokenServer.Decorators;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    public class DefaultDecoratorOutputFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.IdentityServer4.SecureTokenServer.DefaultDecoratorOutputFactoryExtension";
        public override int Order => 0;

        [IntentManaged(Mode.Ignore)]
        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                application.Projects
                    .SelectMany(x => x.TemplateInstances)
                    .OfType<StartupTemplate>()
                    .SelectMany(s => s.GetDecorators())
                    .OfType<IDecoratorExecutionHooks>()
                    .ToList()
                    .ForEach(x => x.BeforeTemplateExecution());
            }
        }
    }
}