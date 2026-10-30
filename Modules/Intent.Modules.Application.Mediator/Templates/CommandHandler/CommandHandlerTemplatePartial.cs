using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Mediator.Templates.CommandHandlerSkill;
using Intent.Modules.Application.Mediator.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates.CommandHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CommandHandlerTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.CommandHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommandHandlerTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            CSharpFile = new CSharpFile(this.GetCqrsNamespace(), this.GetCqrsFolderPath());
            Configure(this);
        }

        // Also called by CommandModelsTemplate, which builds the handler into the command's own file
        // when "Consolidate Command/Query associated files into single file" is on.
        [IntentManaged(Mode.Ignore)]
        internal static void Configure(CSharpTemplateBase<CommandModel> template)
        {
            var model = template.Model;
            template.AddNugetDependency(NugetPackages.MediatorAbstractions(template.OutputTarget));
            template.AddTypeSource(CommandModelsTemplate.TemplateId);
            template.AddTypeSource("Domain.Enum");
            template.AddTypeSource("Application.Contract.Dto");
            template.AddTypeSource("Application.Contract.Enum");
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            ((ICSharpFileBuilderTemplate)template).CSharpFile
                .AddUsing("System")
                .AddClass($"{model.Name}Handler", @class =>
                {
                    var requestType = template.GetTypeName(CommandModelsTemplate.TemplateId, model);
                    var resultType = model.TypeReference.Element != null
                        ? template.GetTypeName(model.TypeReference)
                        : template.UseType("Mediator.Unit");

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface(model.TypeReference.Element != null
                        ? $"{template.UseType("Mediator.ICommandHandler")}<{requestType}, {template.GetTypeName(model.TypeReference)}>"
                        : $"{template.UseType("Mediator.ICommandHandler")}<{requestType}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod($"{template.UseType("System.Threading.Tasks.ValueTask")}<{resultType}>", "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                        method.AddParameter(requestType, "request");
                        method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement("// IntentInitialGen");
                        method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                        method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
                    });
                });
        }

        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.ConsolidateIntoSingleFile();
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig()
                .WithAISummary("Mediator handler implementation for the " + Model.Name + " command.")
                .WithAIContext(
                    $"""
                    Use the `{CommandHandlerSkillTemplate.SkillName}` skill when modifying this handler.
                    """);
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}