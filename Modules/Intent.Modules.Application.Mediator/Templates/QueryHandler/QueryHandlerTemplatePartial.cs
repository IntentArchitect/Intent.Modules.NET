using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Mediator.Templates.QueryHandlerSkill;
using Intent.Modules.Application.Mediator.Templates.QueryModels;
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

namespace Intent.Modules.Application.Mediator.Templates.QueryHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryHandlerTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.QueryHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryHandlerTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(QueryModelsTemplate.TemplateId);
            AddTypeSource("Domain.Enum");
            AddTypeSource("Application.Contract.Dto");
            AddTypeSource("Application.Contract.Enum");
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), this.GetFolderPath(additionalFolders: Model.GetConceptName()))
                .AddUsing("System")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    var requestType = GetTypeName(QueryModelsTemplate.TemplateId, Model);

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface($"{UseType("Mediator.IQueryHandler")}<{requestType}, {GetTypeName(Model.TypeReference)}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod($"{UseType("System.Threading.Tasks.ValueTask")}<{GetTypeName(Model.TypeReference)}>", "Handle", method =>
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
                .WithAISummary("Mediator handler implementation for the " + Model.Name + " query.")
                .WithAIContext(
                    $"""
                    Use the `{QueryHandlerSkillTemplate.SkillName}` skill when modifying this handler.
                    """);
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}