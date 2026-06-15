using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.QueryHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryHandlerTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.QueryHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryHandlerTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(QueryModelsTemplate.TemplateId);
            AddTypeSource("Domain.Enum");
            AddTypeSource("Application.Contract.Dto");
            AddTypeSource("Application.Contract.Enum");
            AddTypeSource("Application.Contracts.Client.Dto");
            AddTypeSource("Application.Contracts.Client.Enum");

            CSharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), this.GetFolderPath(additionalFolders: Model.GetConceptName()))
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod($"Task<{GetTypeName(Model.TypeReference)}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                        method.AddParameter(GetTypeName(QueryModelsTemplate.TemplateId, Model), "query");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement(@"throw new NotImplementedException(""Your implementation here..."");");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
