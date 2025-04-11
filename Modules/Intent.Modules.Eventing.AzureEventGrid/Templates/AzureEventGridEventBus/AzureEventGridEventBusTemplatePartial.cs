using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureEventGridEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureEventGrid.AzureEventGridEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureEventGridEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureMessagingEventGrid(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Transactions")
                .AddUsing("Azure")
                .AddUsing("Azure.Messaging.EventGrid")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"AzureEventGridEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());
                    @class.AddField("List<MessageEntry>", "_messageQueue", field => field.PrivateReadOnly().WithAssignment(new CSharpStatement("[]")));
                    @class.AddField("Dictionary<string, PublisherEntry>", "_lookup", field => field.PrivateReadOnly());

                    @class.NestedClasses.Add(new CSharpRecord("MessageEntry", CSharpFile)
                        .Private()
                        .AddPrimaryConstructor(ctor =>
                        {
                            ctor.AddParameter("object", "Message");
                            ctor.AddParameter(outputTarget.GetProject().NullableEnabled ? "string?" : "string", "Subject");
                        }));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetPublisherOptionsName()}>", "options");

                        ctor.AddStatement("_lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T);
                        method.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        method.AddParameter(T, "message");

                        method.AddStatement("ValidateMessage(message);");
                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, null));");
                    });
                    
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T);
                        method.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        method.AddParameter(T, "message");
                        method.AddParameter("string", "subject");

                        method.AddStatement("ValidateMessage(message);");
                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, subject));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddIfStatement("_messageQueue.Count == 0", fi =>
                        {
                            fi.AddStatement("return;");
                        });

                        method.AddForEachStatement("entry", "_messageQueue", fe =>
                        {
                            fe.AddStatement("var publisherEntry = _lookup[entry.Message.GetType().FullName!];");
                            fe.AddStatement("var client = new EventGridPublisherClient(new Uri(publisherEntry.Endpoint), new AzureKeyCredential(publisherEntry.CredentialKey));");
                            fe.AddStatement("var eventGridEvent = CreateEventGridEvent(entry.Message, entry.Subject);");
                            fe.AddStatement("await client.SendEventAsync(eventGridEvent, cancellationToken);");
                        });
                    });

                    @class.AddMethod("void", "ValidateMessage", method =>
                    {
                        method.Private();
                        method.AddParameter("object", "message");
                        method.AddIfStatement("!_lookup.TryGetValue(message.GetType().FullName!, out _)", ifs =>
                        {
                            ifs.AddStatement("""throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");""");
                        });
                    });

                    @class.AddMethod("EventGridEvent", "CreateEventGridEvent", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("object", "message");
                        method.AddParameter(outputTarget.GetProject().NullableEnabled ? "string?" : "string", "subject");

                        method.AddAssignmentStatement("var eventGridEvent", new CSharpInvocationStatement("new EventGridEvent")
                            .AddArgument("subject", @"subject ?? ""Event""")
                            .AddArgument("eventType", "message.GetType().FullName")
                            .AddArgument("dataVersion", @"""1.0""")
                            .AddArgument("data", "message"));
                        method.AddReturn("eventGridEvent");
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            var templates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Application.Eventing.EventBusInterface"));
            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System");
                    var @interface = file.Interfaces.First();
                    
                    @interface.AddMethod("void", "Publish", m => m
                        .AddGenericParameter("T")
                        .AddParameter("T", "message")
                        .AddParameter("string", "subject")
                        .AddGenericTypeConstraint("T", c => c.AddType("class")));
                });
            }
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