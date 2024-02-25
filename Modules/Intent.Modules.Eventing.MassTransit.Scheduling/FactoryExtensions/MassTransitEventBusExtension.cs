using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Scheduling.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class MassTransitEventBusExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Eventing.MassTransit.Scheduling.MassTransitEventBusExtension";

    [IntentManaged(Mode.Ignore)]
    public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Eventing.MassTransitEventBus"));
        template?.CSharpFile.OnBuild(file =>
        {
            var priClass = file.Classes.First();
            priClass.AddField("List<ScheduleEntry>", "_messagesToSchedule", field => field
                .PrivateReadOnly()
                .WithAssignment("new List<ScheduleEntry>()"));

            priClass.AddMethod("void", "SchedulePublish", method =>
            {
                method.AddGenericParameter("T", out var T)
                    .AddGenericTypeConstraint(T, c => c.AddType("class"))
                    .AddParameter(T, "message")
                    .AddParameter("DateTime", "scheduled")
                    .AddStatement("_messagesToSchedule.Add(ScheduleEntry.ForScheduled(message, scheduled));");
            });

            priClass.AddMethod("void", "SchedulePublish", method =>
            {
                method.AddGenericParameter("T", out var T)
                    .AddGenericTypeConstraint(T, c => c.AddType("class"))
                    .AddParameter(T, "message")
                    .AddParameter("TimeSpan", "delay")
                    .AddStatement("_messagesToSchedule.Add(ScheduleEntry.ForDelay(message, delay));");
            });

            priClass.FindMethod("FlushAllAsync")?.AddStatement("_messagesToSchedule.Clear();");

            priClass.FindMethod("PublishWithConsumeContext")
                ?.AddForEachStatement("scheduleEntry", "_messagesToSchedule", loop =>
                {
                    loop.SeparatedFromPrevious();
                    loop.AddStatement(
                        "await ConsumeContext!.SchedulePublish(scheduleEntry.Scheduled, scheduleEntry.Message, cancellationToken).ConfigureAwait(false);");
                });

            priClass.FindMethod("PublishWithNormalContext")
                ?.AddStatement("var messageScheduler = _serviceProvider.GetRequiredService<IMessageScheduler>();",
                    s => s.SeparatedFromPrevious())
                .AddForEachStatement("scheduleEntry", "_messagesToSchedule", loop =>
                {
                    loop.SeparatedFromPrevious();
                    loop.AddStatement(
                        "await messageScheduler.SchedulePublish(scheduleEntry.Scheduled, scheduleEntry.Message, cancellationToken).ConfigureAwait(false);");
                });

            @priClass.AddNestedClass("ScheduleEntry", nested =>
            {
                nested.Private();
                nested.AddConstructor(ctor =>
                {
                    ctor.Private();
                    ctor.AddParameter("object", "message", param => param.IntroduceProperty(prop => prop.ReadOnly()));
                    ctor.AddParameter("DateTime", "scheduled", param => param.IntroduceProperty(prop => prop.ReadOnly()));
                });
                nested.AddMethod("ScheduleEntry", "ForScheduled", method =>
                {
                    method.Static();
                    method.AddParameter("object", "message");
                    method.AddParameter("DateTime", "scheduled");
                    method.AddStatement("return new ScheduleEntry(message, scheduled);");
                });
                nested.AddMethod("ScheduleEntry", "ForDelay", method =>
                {
                    method.Static();
                    method.AddParameter("object", "message");
                    method.AddParameter("TimeSpan", "delay");
                    method.AddStatement("return new ScheduleEntry(message, DateTime.UtcNow.Add(delay));");
                });
            });
        }, 10);
    }
}