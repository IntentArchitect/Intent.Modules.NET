using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates.SequentialNotificationPublisher
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SequentialNotificationPublisherTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.SequentialNotificationPublisher";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SequentialNotificationPublisherTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Mediator")
                .AddClass("SequentialNotificationPublisher", @class =>
                {
                    @class.Sealed();
                    @class.ImplementsInterface("INotificationPublisher");
                    @class.WithComments(
                    [
                        "/// <summary>",
                        "/// Runs notification handlers one at a time and stops at the first failure, rethrowing the",
                        "/// handler's own exception (MediatR's default semantics). Mediator's built-in",
                        "/// <see cref=\"ForeachAwaitPublisher\"/> keeps running the remaining handlers and wraps failures",
                        "/// in an <see cref=\"AggregateException\"/>, which API exception mapping turns into a 500.",
                        "/// </summary>"
                    ]);
                    @class.AddMethod("ValueTask", "Publish", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddGenericParameter("TNotification", out var tNotification);
                        method.AddParameter($"NotificationHandlers<{tNotification}>", "handlers");
                        method.AddParameter(tNotification, "notification");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddGenericTypeConstraint(tNotification, c => c.AddType("INotification"));
                        method.AddForEachStatement("handler", "handlers", loop =>
                        {
                            loop.AddStatement("await handler.Handle(notification, cancellationToken);");
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
