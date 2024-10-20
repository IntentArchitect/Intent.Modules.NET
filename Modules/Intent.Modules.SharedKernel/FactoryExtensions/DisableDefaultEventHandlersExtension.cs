using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.SharedKernel.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DisableDefaultEventHandlersExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SharedKernel.DisableDefaultEventHandlersExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //Only supporting Explicit Event Subscriptions, as events may be defined here to be cconsumed outside the Shared Kernel
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.MediatR.DomainEvents.DefaultDomainEventHandler");
            foreach (var template in templates)
            {
                template.CanRun = false;
            }

            var readme = application.FindTemplateInstance<IntentTemplateBase>("Intent.EntityFrameworkCore.DbMigrationsReadMe");
            readme.CanRun = false;
            
        }
    }
}