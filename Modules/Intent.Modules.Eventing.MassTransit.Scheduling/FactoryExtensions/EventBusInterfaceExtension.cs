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

namespace Intent.Modules.Eventing.MassTransit.Scheduling.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EventBusInterfaceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.Scheduling.EventBusInterfaceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Application.Eventing.EventBusInterface"));
            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var iface = file.Interfaces.First();
                    iface.AddMethod("void", "SchedulePublish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddParameter("DateTime", "scheduled");
                    });
                    iface.AddMethod("void", "SchedulePublish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddParameter("TimeSpan", "delay");
                    });
                });
            }
        }
    }
}