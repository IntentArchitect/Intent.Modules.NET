using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.CommandHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class CommandHandlerTemplate : CSharpTemplateBase<CommandModel, CommandHandlerDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.CommandHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandHandlerTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource(TemplateFulfillingRoles.Domain.Enum, "System.Collections.Generic.List<{0}>");
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto, "System.Collections.Generic.List<{0}>");
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Enum, "System.Collections.Generic.List<{0}>");

            CSharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), "")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MediatR")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    @class.AddMetadata("model", Model);
                    @class.WithBaseType(GetRequestHandlerInterface());
                    @class.AddAttribute("IntentManaged(Mode.Merge, Signature = Mode.Fully)");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute("IntentManaged(Mode.Merge)");
                    });
                    @class.AddMethod(GetReturnType(), "Handle", method =>
                    {
                        method.TryAddXmlDocComments(Model.InternalElement);
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        method.AddParameter(GetCommandModelName(), "request");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement($@"throw new NotImplementedException(""Your implementation here..."");");
                    });
                });
        }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetRequestHandlerInterface()
        {
            return Model.TypeReference.Element != null
                ? $"IRequestHandler<{GetCommandModelName()}, {GetTypeName(Model.TypeReference)}>"
                : $"IRequestHandler<{GetCommandModelName()}>";
        }

        private string GetCommandModelName()
        {
            return GetTypeName(CommandModelsTemplate.TemplateId, Model);
        }

        private string GetReturnType()
        {
            return Model.TypeReference.Element != null
                ? $"Task<{GetTypeName(Model.TypeReference)}>"
                : "Task";
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "2.0"), new Mediator12Migration());
        }

        private class Mediator12Migration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                return currentText.Replace(@"return Unit.Value;\r\n", "")
                    .Replace(@"return Unit.Value;\n", "")
                    .Replace(@"return Unit.Value;", "");
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
        }
    }
}