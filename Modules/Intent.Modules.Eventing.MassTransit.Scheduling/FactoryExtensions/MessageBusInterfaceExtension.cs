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
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Eventing.MessageBusInterface));
            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var iface = file.Interfaces.First();
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
                        method.AddStatements(
                            """
                            Publish<TMessage>(message, new Dictionary<string, object>
                            {
                                { "scheduled", scheduled }
                            });
                            """.ConvertToStatements());
                    });
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
                        method.AddStatements(
                            """
                            Publish<TMessage>(message, new Dictionary<string, object>
                            {
                                { "scheduled", DateTime.UtcNow.Add(delay) }
                            });
                            """.ConvertToStatements());
                    });
                }, 2);
            }
        }
    }
}