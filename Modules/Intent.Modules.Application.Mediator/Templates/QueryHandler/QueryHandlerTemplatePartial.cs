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
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            CSharpFile = new CSharpFile(this.GetCqrsNamespace(), this.GetCqrsFolderPath());
            Configure(this);
        }

        // Also called by QueryModelsTemplate, which builds the handler into the query's own file
        // when "Consolidate Command/Query associated files into single file" is on.
        [IntentManaged(Mode.Ignore)]
        internal static void Configure(CSharpTemplateBase<QueryModel> template)
        {
            var model = template.Model;
            template.AddNugetDependency(NugetPackages.MediatorAbstractions(template.OutputTarget));
            template.AddTypeSource(QueryModelsTemplate.TemplateId);
            template.AddTypeSource("Domain.Enum");
            template.AddTypeSource("Application.Contract.Dto");
            template.AddTypeSource("Application.Contract.Enum");
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            template.AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            ((ICSharpFileBuilderTemplate)template).CSharpFile
                .AddUsing("System")
                .AddClass($"{model.Name}Handler", @class =>
                {
                    var requestType = template.GetTypeName(QueryModelsTemplate.TemplateId, model);

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface($"{template.UseType("Mediator.IQueryHandler")}<{requestType}, {template.GetTypeName(model.TypeReference)}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod($"{template.UseType("System.Threading.Tasks.ValueTask")}<{template.GetTypeName(model.TypeReference)}>", "Handle", method =>
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