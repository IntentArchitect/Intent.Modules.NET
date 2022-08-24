using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.EventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.Consumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ConsumerTemplate : CSharpTemplateBase<MessageHandlerModel>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.Consumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ConsumerTemplate(IOutputTarget outputTarget, MessageHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MassTransitAbstractions);
            AddTypeSource(EventMessageTemplate.TemplateId);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.TypeReference.Element.Name.ToPascalCase()}Consumer",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private bool UseExplicitNullSymbol => Project.GetProject().NullableEnabled;

        private string GetMessageName()
        {
            return GetTypeName(Model.TypeReference);
        }

        private string GetClassMembers()
        {
            var members = new List<string>();

            members.Add($@"private readonly IServiceProvider _serviceProvider;");

            if (!members.Any())
            {
                return string.Empty;
            }

            const string newLine = @"
        ";
            return newLine + string.Join(newLine, members) + newLine;
        }

        private string GetConstructorParameters()
        {
            var parameters = new List<string>();

            parameters.Add($@"IServiceProvider serviceProvider");

            return string.Join(", ", parameters);
        }

        private string GetConstructorImplementation()
        {
            var statements = new List<string>();

            statements.Add($@"_serviceProvider = serviceProvider;");

            if (!statements.Any())
            {
                return string.Empty;
            }

            const string newLine = @"
            ";
            return newLine + string.Join(newLine, statements);
        }

        private string GetClassMethods()
        {
            var lines = new List<string>();

            lines.Add(@$"public async Task Consume(ConsumeContext<{GetMessageName()}> context)");
            lines.Add(@$"{{");
            lines.Add(@$"    using var scope = _serviceProvider.CreateScope();");
            lines.Add(@$"    ");
            lines.Add($@"    var handler = scope.ServiceProvider.GetService<{this.GetEventHandlerInterfaceName(Model)}>(){(UseExplicitNullSymbol ? "!" : string.Empty)};");
            lines.Add(@$"    var messagePublishContext = scope.ServiceProvider.GetService<{this.GetMessagePublishContextName()}>(){(UseExplicitNullSymbol ? "!" : string.Empty)};");
            lines.Add($@"    messagePublishContext.Current = context;");
            lines.Add(@$"    ");
            lines.Add(@$"    await handler.HandleAsync(context.Message, context.CancellationToken);");
            lines.Add(@$"    ");
            lines.Add(@$"    await messagePublishContext.FlushAllAsync(context.CancellationToken);");
            lines.Add(@$"}}");
            lines.Add(@$"");

            const string newLine = @"
        ";
            return newLine + string.Join(newLine, lines);
        }
    }
}