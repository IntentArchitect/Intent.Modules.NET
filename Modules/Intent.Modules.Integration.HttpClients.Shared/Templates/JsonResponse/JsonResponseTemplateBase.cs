using System.Linq;
using Intent.Engine;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.JsonResponse
{
    public class JsonResponseTemplateBase : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
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

        public override bool CanRunTemplate()
        {
            return ExecutionContext
                .MetadataManager
                .ServiceProxies(ExecutionContext.GetApplicationConfig().Id)
                .GetServiceProxyModels()
                .SelectMany(s => s.GetMappedEndpoints())
                .Any(x => x.MediaType == HttpMediaType.ApplicationJson);
        }
    }
}