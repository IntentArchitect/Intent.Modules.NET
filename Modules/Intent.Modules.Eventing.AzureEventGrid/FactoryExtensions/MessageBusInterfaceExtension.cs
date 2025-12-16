using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusInterfaceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.AzureEventGrid.MessageBusInterfaceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // Add method signatures to MessageBusInterface
            var interfaceTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Eventing.MessageBusInterface));
            foreach (var template in interfaceTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var iface = file.Interfaces.First();
                    
                    // Publish with additional data
                    iface.AddMethod("void", "Publish", method =>
                    {
                        method.WithComments([
                            "/// <summary>",
                            "/// Queues a message to be published with additional metadata (Azure Event Grid-specific extension attributes).",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to publish.</param>",
                            "/// <param name=\"additionalData\">Additional metadata to include with the message (e.g., Subject, extension attributes).</param>",
                            "/// <remarks>",
                            "/// The message is buffered until <see cref=\"FlushAllAsync\"/> is invoked. Providers that do not support additional data may ignore this overload.",
                            "/// </remarks>"
                        ]);
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.Collections.Generic.IDictionary") + "<string, object>", "additionalData");
                    });
                    
                    // Send with additional data
                    iface.AddMethod("void", "Send", method =>
                    {
                        method.WithComments([
                            "/// <summary>",
                            "/// Queues a point-to-point message with additional metadata (Azure Event Grid-specific extension attributes).",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to send.</param>",
                            "/// <param name=\"additionalData\">Additional metadata to include with the message (e.g., Subject, extension attributes).</param>",
                            "/// <remarks>",
                            "/// The message is buffered until <see cref=\"FlushAllAsync\"/> is invoked. Providers that do not support additional data may ignore this overload.",
                            "/// </remarks>"
                        ]);
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.Collections.Generic.IDictionary") + "<string, object>", "additionalData");
                    });
                }, 1);
            }

            // Add implementations to MessageBusImplementation templates
            var implementationTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Eventing.MessageBusImplementation));
            foreach (var template in implementationTemplates)
            {
                var isAzureEventGrid = template.Id == "Intent.Eventing.AzureEventGrid.AzureEventGridMessageBus";
                
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    
                    // Publish with additional data
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.Collections.Generic.IDictionary") + "<string, object>", "additionalData");
                        
                        if (isAzureEventGrid)
                        {
                            // Azure Event Grid already has the implementation
                            method.AddStatement("_messageQueue.Add(new MessageEntry(message, additionalData));");
                        }
                        else
                        {
                            method.AddStatement("throw new NotSupportedException(\"Publishing with additional data is not supported by this message bus provider.\");");
                        }
                    });
                    
                    // Send with additional data
                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.Collections.Generic.IDictionary") + "<string, object>", "additionalData");
                        
                        if (isAzureEventGrid)
                        {
                            // Azure Event Grid already has the implementation
                            method.AddStatement("_messageQueue.Add(new MessageEntry(message, additionalData));");
                        }
                        else
                        {
                            method.AddStatement("throw new NotSupportedException(\"Sending with additional data is not supported by this message bus provider.\");");
                        }
                    });
                }, 2);
            }

            // Add methods to CompositeMessageBus
            var compositeTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.CompositeMessageBus");
            if (compositeTemplate != null)
            {
                compositeTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    
                    // Publish with additional data
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(compositeTemplate.UseType("System.Collections.Generic.IDictionary") + "<string, object>", "additionalData");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Publish(message, additionalData));");
                    });
                    
                    // Send with additional data
                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(compositeTemplate.UseType("System.Collections.Generic.IDictionary") + "<string, object>", "additionalData");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Send(message, additionalData));");
                    });
                }, 3);
            }
        }
    }
}