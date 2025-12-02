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

namespace Intent.Modules.Eventing.MassTransit.Scheduling.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusInterfaceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.Scheduling.MessageBusInterfaceExtension";

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
                    // Publish with DateTime scheduled
                    iface.AddMethod("void", "Publish", method =>
                    {
                        method.WithComments([
                            "/// <summary>",
                            "/// Queues a message to be published at a specific scheduled UTC time.",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to publish.</param>",
                            "/// <param name=\"scheduled\">The UTC date/time when the message should be dispatched (MassTransit scheduling capability).</param>",
                            "/// <remarks>",
                            "/// Scheduling is a provider-specific feature; if the underlying implementation does not support scheduling this overload may be ignored.",
                            "/// The message will be buffered until <see cref=\"FlushAllAsync\"/> is invoked (at which point the schedule instruction is applied by the provider).",
                            "/// </remarks>"
                        ]);
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.DateTime"), "scheduled");
                    });
                    
                    // Publish with TimeSpan delay
                    iface.AddMethod("void", "Publish", method =>
                    {
                        method.WithComments([
                            "/// <summary>",
                            "/// Queues a message to be published after a delay relative to the current UTC time.",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to publish.</param>",
                            "/// <param name=\"delay\">The time span to add to the current UTC time for scheduling (MassTransit scheduling capability).</param>",
                            "/// <remarks>",
                            "/// Scheduling is provider-specific; if unsupported this overload may be ignored. The actual dispatch occurs when <see cref=\"FlushAllAsync\"/> is called and the provider processes the schedule metadata.",
                            "/// </remarks>"
                        ]);
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.TimeSpan"), "delay");
                    });
                }, 2);
            }

            // Add implementations to MessageBusImplementation templates
            var implementationTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Eventing.MessageBusImplementation));
            foreach (var template in implementationTemplates)
            {
                var isMassTransit = template.Id == "Intent.Eventing.MassTransit.MassTransitMessageBus";
                
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    
                    // Publish with DateTime scheduled
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.DateTime"), "scheduled");
                        
                        if (isMassTransit)
                        {
                            method.AddStatements(
                                """
                                Publish<TMessage>(message, new Dictionary<string, object>
                                {
                                    { "scheduled", scheduled }
                                });
                                """.ConvertToStatements());
                        }
                        else
                        {
                            method.AddStatement("throw new NotSupportedException(\"Scheduled publishing is not supported by this message bus provider.\");");
                        }
                    });
                    
                    // Publish with TimeSpan delay
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.TimeSpan"), "delay");
                        
                        if (isMassTransit)
                        {
                            method.AddStatements(
                                """
                                Publish<TMessage>(message, new Dictionary<string, object>
                                {
                                    { "scheduled", DateTime.UtcNow.Add(delay) }
                                });
                                """.ConvertToStatements());
                        }
                        else
                        {
                            method.AddStatement("throw new NotSupportedException(\"Scheduled publishing is not supported by this message bus provider.\");");
                        }
                    });
                }, 3);
            }

            // Add methods to CompositeMessageBus
            var compositeTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.CompositeMessageBus");
            if (compositeTemplate != null)
            {
                compositeTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    
                    // Publish with DateTime scheduled
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(compositeTemplate.UseType("System.DateTime"), "scheduled");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Publish(message, scheduled));");
                    });
                    
                    // Publish with TimeSpan delay
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(compositeTemplate.UseType("System.TimeSpan"), "delay");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Publish(message, delay));");
                    });
                }, 4);
            }
        }
    }
}