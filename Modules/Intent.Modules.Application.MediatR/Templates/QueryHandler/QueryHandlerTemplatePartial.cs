using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class QueryHandlerTemplate : CSharpTemplateBase<QueryModel, QueryHandlerDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.QueryHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryHandlerTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
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
                        ctor.AddAttribute("IntentManaged(Mode.Ignore)");
                    });
                    @class.AddMethod($"Task<{GetTypeName(Model.TypeReference)}>", "Handle", method =>
                    {
                        method.TryAddXmlDocComments(Model.InternalElement);
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        method.AddParameter(GetQueryModelName(), "request");
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
                ? $"IRequestHandler<{GetQueryModelName()}, {GetTypeName(Model.TypeReference)}>"
                : $"IRequestHandler<{GetQueryModelName()}>";
        }

        private string GetQueryModelName()
        {
            return GetTypeName(QueryModelsTemplate.TemplateId, Model);
        }
    }
}