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
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Azure")
                .AddUsing("Azure.Messaging")
                .AddUsing("Azure.Messaging.EventGrid")
                .AddUsing("Microsoft.Extensions.Options")
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
                            ctor.AddParameter(outputTarget.GetProject().NullableEnabled ? "IDictionary<string, object>?" : "IDictionary<string, object>", "AdditionalData");
                        }));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetAzureEventGridPublisherOptionsName()}>", "options");
                        ctor.AddParameter(this.GetAzureEventGridPublisherPipelineName(), "pipeline", param => param.IntroduceReadonlyField());

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
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("ValidateMessage(message);");
                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, additionalData));");
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
                            fe.AddStatement("var cloudEvent = CreateCloudEvent(entry, publisherEntry);");
                            fe.AddStatement(new CSharpAwaitExpression(new CSharpInvocationStatement("_pipeline.ExecuteAsync")
                                .AddArgument("cloudEvent")
                                .AddArgument(new CSharpLambdaBlock("async (@event, token)")
                                    .AddStatement("await client.SendEventAsync(@event, token);")
                                    .AddStatement("return @event;")
                                )
                                .AddArgument("cancellationToken")
                            ));
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

                    @class.AddMethod("CloudEvent", "CreateCloudEvent", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("MessageEntry", "messageEntry");
                        method.AddParameter("PublisherEntry", "publisherEntry");

                        method.AddStatement(new CSharpAssignmentStatement(new CSharpVariableDeclaration("cloudEvent"), new CSharpInvocationStatement("new CloudEvent")
                            .AddArgument("source", @"publisherEntry.Source")
                            .AddArgument("type", "messageEntry.Message.GetType().FullName!")
                            .AddArgument("jsonSerializableData", @"messageEntry.Message")));

                        method.AddIfStatement("messageEntry.AdditionalData is not null", extensionAttributesBlock =>
                        {
                            extensionAttributesBlock.AddIfStatement(@"messageEntry.AdditionalData.TryGetValue(""Subject"", out var subject)", subjectFoundBlock =>
                            {
                                subjectFoundBlock.AddStatement("cloudEvent.Subject = (string)subject;");
                            });
                            extensionAttributesBlock.AddForEachStatement("extensionAttribute", @"messageEntry.AdditionalData.Where(p => p.Key != ""Subject"")", forEachAttributeBlock =>
                            {
                                forEachAttributeBlock.AddStatement("cloudEvent.ExtensionAttributes.Add(extensionAttribute.Key, extensionAttribute.Value);");
                            });
                        });
                        
                        method.AddReturn("cloudEvent");
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
                    file.AddUsing("System.Collections.Generic");
                    var @interface = file.Interfaces.First();
                    
                    @interface.AddMethod("void", "Publish", m => m
                        .AddGenericParameter("T")
                        .AddParameter("T", "message")
                        .AddParameter("IDictionary<string, object>", "additionalData")
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