using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NServiceBusMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.NServiceBus.NServiceBusMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("NServiceBus")
                .AddClass("NServiceBusMessageBus", @class =>
                {
                    @class.ImplementsInterface(this.GetBusInterfaceName());

                    @class.AddField("List<(object Message, bool IsPublish)>", "_buffer", field => field
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IMessageSession", "messageSession", param =>
                            param.IntroduceReadonlyField());
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage)
                            .AddGenericTypeConstraint(tMessage, c => c.AddType("class"))
                            .AddParameter(tMessage, "message");
                        method.AddStatement("_buffer.Add((message, true));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage)
                            .AddGenericTypeConstraint(tMessage, c => c.AddType("class"))
                            .AddParameter(tMessage, "message");
                        method.AddStatement("_buffer.Add((message, false));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", p =>
                            p.WithDefaultValue("default"));
                        method.AddForEachStatement("(message, isPublish)", "_buffer", fe =>
                        {
                            fe.AddIfStatement("isPublish", b =>
                                b.AddStatement("await _messageSession.Publish(message);"));
                            fe.AddElseStatement(b =>
                                b.AddStatement("await _messageSession.Send(message);"));
                        });
                        method.AddStatement("_buffer.Clear();", s => s.SeparatedFromPrevious());
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(this.GetBusInterfaceName())
                .ForConcern("Infrastructure")
                .WithPriority(100)
                .WithPerServiceCallLifeTime());
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