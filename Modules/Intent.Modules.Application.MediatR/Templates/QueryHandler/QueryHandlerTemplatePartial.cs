using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
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


            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            CSharpFile = new CSharpFile($"{this.GetQueryNamespace()}", $"{this.GetQueryFolderPath()}");
            Configure(this, model);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile();
        }

        public static void Configure(ICSharpFileBuilderTemplate template, QueryModel model)
        {
            template.AddNugetDependency(NugetPackages.MediatR(template.OutputTarget));
            template.AddTypeSource(TemplateRoles.Application.Query);
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
                    @class.AddMethod($"Task<{template.GetTypeName(model.TypeReference)}>", "Handle", method =>
                    {
                        method.RegisterAsProcessingHandlerForModel(model);
                        method.TryAddXmlDocComments(model.InternalElement);
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        method.AddParameter(GetQueryModelName(template, model), "request");
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

        private static string GetRequestHandlerInterface(ICSharpTemplate template, QueryModel model)
        {
            return model.TypeReference.Element != null
                ? $"IRequestHandler<{GetQueryModelName(template, model)}, {template.GetTypeName(model.TypeReference)}>"
                : $"IRequestHandler<{GetQueryModelName(template, model)}>";
        }

        private static string GetQueryModelName(ICSharpTemplate template, QueryModel model)
        {
            return template.GetTypeName(QueryModelsTemplate.TemplateId, model);
        }
    }
}