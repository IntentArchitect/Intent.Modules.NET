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

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusInterfaceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.MessageBusInterfaceExtension";

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
                    iface.AddMethod("void", "Send", method =>
                    {
                        method.WithComments([
                            "/// <summary>",
                            "/// Queues a point-to-point message for dispatch to a specific broker address.",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to send.</param>",
                            "/// <param name=\"address\">The destination address understood by providers that support explicit addressing (MassTransit-specific concept).</param>",
                            "/// <remarks>",
                            "/// The message is buffered until <see cref=\"FlushAllAsync\"/> is invoked. Providers that do not support explicit addressing may ignore this overload.",
                            "/// </remarks>"
                        ]);
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter(template.UseType("System.Uri"), "address");
                        method.AddStatements(
                            """
                            Send<TMessage>(message, new Dictionary<string, object>
                            {
                                { "address", address.ToString() }
                            });
                            """.ConvertToStatements());
                    });
                }, 1);
            }
        }
    }
}