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
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(CommandModelsTemplate.TemplateId);
            AddTypeSource("Domain.Enum");
            AddTypeSource("Application.Contract.Dto");
            AddTypeSource("Application.Contract.Enum");
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), this.GetFolderPath(additionalFolders: Model.GetConceptName()))
                .AddUsing("System")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    var requestType = GetTypeName(CommandModelsTemplate.TemplateId, Model);
                    var resultType = Model.TypeReference.Element != null
                        ? GetTypeName(Model.TypeReference)
                        : UseType("Mediator.Unit");

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface(Model.TypeReference.Element != null
                        ? $"{UseType("Mediator.ICommandHandler")}<{requestType}, {GetTypeName(Model.TypeReference)}>"
                        : $"{UseType("Mediator.ICommandHandler")}<{requestType}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod($"{UseType("System.Threading.Tasks.ValueTask")}<{resultType}>", "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                        method.AddParameter(requestType, "request");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement("// IntentInitialGen");
                        method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                        method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
                    });
                });
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