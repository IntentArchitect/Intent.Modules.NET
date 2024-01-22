using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EventBusInterfaceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.EventBusInterfaceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Application.Eventing.EventBusInterface"));
            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System");
                    var @interface = file.Interfaces.First();
                    @interface.AddMethod("void", "Send", m => m
                        .AddGenericParameter("T")
                        .AddParameter("T", "message")
                        .AddGenericTypeConstraint("T", c => c.AddType("class")));
                    @interface.AddMethod("void", "Send", m => m
                        .AddGenericParameter("T")
                        .AddParameter("T", "message")
                        .AddParameter("Uri", "address")
                        .AddGenericTypeConstraint("T", c => c.AddType("class")));
                });
            }
        }
    }
}