using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.JsonResponse
{
    public abstract class JsonResponseTemplateBase : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        protected JsonResponseTemplateBase(string templateId, IOutputTarget outputTarget, object model = null) : base(templateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .IntentManagedFully()
                .AddClass("JsonResponse", @class => @class
                    .WithComments(new[]
                    {
                        "/// <summary>",
                        "/// Implicit wrapping of types that serialize to non-complex values.",
                        "/// </summary>",
                        "/// <typeparam name=\"T\">Types such as string, Guid, int, long, etc.</typeparam>"
                    })
                    .AddGenericParameter("T")
                    .AddConstructor(c => c
                        .AddParameter("T", "value", p => p.IntroduceProperty())
                    )
                );
        }

        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

        public override string TransformText() => CSharpFile.ToString();

        protected abstract IEnumerable<IDesigner> GetSourceDesigners(IMetadataManager metadataManager, string applicationId);

        public override bool CanRunTemplate()
        {
            return GetSourceDesigners(ExecutionContext.MetadataManager, ExecutionContext.GetApplicationConfig().Id)
                .SelectMany(s => s.GetServiceProxyModels())
                .SelectMany(s => s.GetMappedEndpoints())
                .Any(x => x.MediaType == HttpMediaType.ApplicationJson);
        }
    }
}