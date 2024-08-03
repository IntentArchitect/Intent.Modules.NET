using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
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
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            CSharpFile = new CSharpFile($"{this.GetCommandNamespace()}", $"{this.GetCommandFolderPath()}");
            Configure(this, model);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile();
        }

        internal static void Configure(ICSharpFileBuilderTemplate template, CommandModel model)
        {
            template.AddNugetDependency(NugetPackages.MediatR(template.OutputTarget));
            template.AddTypeSource(TemplateRoles.Application.Command);
            template.AddTypeSource(TemplateRoles.Domain.Enum);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            template.CSharpFile
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MediatR")
                .AddClass($"{model.Name}Handler", @class =>
                {
                    @class.AddMetadata("handler", true);
                    @class.AddMetadata("model", model);
                    @class.ImplementsInterface(GetRequestHandlerInterface(template, model));
                    @class.AddAttribute("IntentManaged(Mode.Merge, Signature = Mode.Fully)");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod(GetReturnType(template, model), "Handle", method =>
                    {
                        method.RegisterAsProcessingHandlerForModel(model);
                        method.TryAddXmlDocComments(model.InternalElement);
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        method.AddParameter(GetCommandModelName(template, model), "request", p => p.RepresentsModel(model));
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
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

        private static string GetRequestHandlerInterface(ICSharpTemplate template, CommandModel model)
        {
            return model.TypeReference.Element != null
                ? $"IRequestHandler<{GetCommandModelName(template, model)}, {template.GetTypeName(model.TypeReference)}>"
                : $"IRequestHandler<{GetCommandModelName(template, model)}>";
        }

        private static string GetCommandModelName(ICSharpTemplate template, CommandModel model)
        {
            return template.GetTypeName(CommandModelsTemplate.TemplateId, model);
        }

        private static string GetReturnType(ICSharpTemplate template, CommandModel model)
        {
            return model.TypeReference.Element != null
                ? $"Task<{template.GetTypeName(model.TypeReference)}>"
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