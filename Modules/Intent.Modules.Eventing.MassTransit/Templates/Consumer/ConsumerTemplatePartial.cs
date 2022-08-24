using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
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
        public const string TemplateId = "Intent.Modules.Eventing.MassTransit.Consumer";

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

        private string GetMessageName()
        {
            return GetTypeName(Model.TypeReference);
        }

        private string GetClassMembers()
        {
            var members = new List<string>();

            members.Add($@"private readonly {this.GetEventHandlerInterfaceName(Model)} _eventHandler;");

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

            parameters.Add($@"{this.GetEventHandlerInterfaceName(Model)} eventHandler");

            return string.Join(", ", parameters);
        }

        private string GetConstructorImplementation()
        {
            var statements = new List<string>();

            statements.Add($@"_eventHandler = eventHandler;");

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
            lines.Add($@"    _eventHandler.Handle(context.Message);");
            lines.Add(@$"}}");
            lines.Add(@$"");

            const string newLine = @"
        ";
            return newLine + string.Join(newLine, lines);
        }
    }
}