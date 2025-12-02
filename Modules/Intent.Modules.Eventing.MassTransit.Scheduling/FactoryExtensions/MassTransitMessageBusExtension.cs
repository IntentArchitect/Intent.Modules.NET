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
public class MassTransitMessageBusExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Eventing.MassTransit.Scheduling.MassTransitMessageBusExtension";

    [IntentManaged(Mode.Ignore)]
    public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Eventing.MassTransitEventBus"));
        template?.CSharpFile.OnBuild(file =>
        {
            var priClass = file.Classes.First();
            // Ensure ScheduledKey constant exists
            priClass.AddField("string", "ScheduledKey", f => f.Private().Constant(@"""scheduled"""));

            // Modify Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData) to set DispatchType based on ScheduledKey
            var publishWithAdditionalData = priClass.FindMethod(m => m.Name == "Publish" && m.Parameters.Count == 2 && m.Parameters[1].Type == "IDictionary<string, object>");
            if (publishWithAdditionalData is not null)
            {
                publishWithAdditionalData.Statements.Clear();
                publishWithAdditionalData.AddStatement("_messagesToDispatch.Add(new MessageEntry(message, additionalData, additionalData.ContainsKey(ScheduledKey) ? DispatchType.Schedule : DispatchType.Publish));");
            }

            // In FlushAllAsync, add messagesToSchedule and invoke SchedulePublishAsync
            var flushMethod = priClass.FindMethod("FlushAllAsync");
            flushMethod?.AddStatement("var messagesToSchedule = _messagesToDispatch.Where(x => x.DispatchType == DispatchType.Schedule).ToList();", s => s.SeparatedFromPrevious());
            flushMethod?.AddStatement("await SchedulePublishAsync(messagesToSchedule, cancellationToken).ConfigureAwait(false);");

            // Add DispatchType.Schedule to DispatchType enum
            var dispatchEnum = priClass.NestedEnums.FirstOrDefault(x => x.Name == "DispatchType");
            dispatchEnum?.AddLiteral("Schedule");

            // Add SchedulePublishAsync method implementation
            priClass.AddMethod("Task", "SchedulePublishAsync", method =>
            {
                method.Private().Async();
                method.AddParameter("List<MessageEntry>", "messagesToSchedule");
                method.AddParameter("CancellationToken", "cancellationToken");

                method.AddIfStatement("!messagesToSchedule.Any()", block =>
                {
                    block.AddReturn("");
                });

                method.AddIfStatement("ConsumeContext is not null", block =>
                {
                    block.AddForEachStatement("toSchedule", "messagesToSchedule", fe =>
                    {
                        fe.AddIfStatement("toSchedule.AdditionalData != null && toSchedule.AdditionalData.TryGetValue(ScheduledKey, out var scheduledObj) && scheduledObj is DateTime scheduled", b =>
                        {
                            b.AddStatement("await ConsumeContext!.SchedulePublish(scheduled, toSchedule.Message, cancellationToken).ConfigureAwait(false);");
                        });
                    });
                    block.AddReturn("");
                });

                method.AddStatement("var messageScheduler = _serviceProvider.GetRequiredService<IMessageScheduler>();");
                method.AddForEachStatement("toSchedule", "messagesToSchedule", fe =>
                {
                    fe.AddIfStatement("toSchedule.AdditionalData != null && toSchedule.AdditionalData.TryGetValue(ScheduledKey, out var scheduledObj) && scheduledObj is DateTime scheduled", b =>
                    {
                        b.AddStatement("await messageScheduler.SchedulePublish(scheduled, toSchedule.Message, cancellationToken).ConfigureAwait(false);");
                    });
                });
            });
        }, 10);
    }
}