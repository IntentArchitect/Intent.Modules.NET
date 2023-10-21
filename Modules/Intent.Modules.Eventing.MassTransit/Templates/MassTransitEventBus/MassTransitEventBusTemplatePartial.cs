using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MassTransitEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MassTransitEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MassTransit")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("MassTransitEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());
                    @class.AddField("List<object>", "_messagesToPublish", field => field
                        .PrivateReadOnly()
                        .WithAssignment("new List<object>()"));

                    @class.AddConstructor(ctor => { ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField()); });

                    @class.AddProperty("ConsumeContext?", "ConsumeContext");

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddStatement("_messagesToPublish.Add(message);");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddIfStatement("ConsumeContext is not null", block =>
                        {
                            block.AddStatement("await PublishWithConsumeContext(cancellationToken);");
                        });
                        method.AddElseStatement(block =>
                        {
                            block.AddStatement("await PublishWithNormalContext(cancellationToken);");
                        });
                        method.AddStatement("_messagesToPublish.Clear();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("Task", "PublishWithConsumeContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("await ConsumeContext!.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);");
                    });

                    @class.AddMethod("Task", "PublishWithNormalContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();");

                        method.AddStatement("await publishEndpoint.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);",
                            s => s.SeparatedFromPrevious());
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