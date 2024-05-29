using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;

public abstract class PagedResultTemplateBase : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TypeDefinitionElementId = "9204e067-bdc8-45e7-8970-8a833fdc5253";
    
    private readonly IServiceProxyMappedService _serviceProxyMappedService;

    protected PagedResultTemplateBase(
        string templateId, 
        IOutputTarget outputTarget,
        IServiceProxyMappedService serviceProxyMappedService) : base(templateId, outputTarget, new MetadataModel(TypeDefinitionElementId))
    {
        _serviceProxyMappedService = serviceProxyMappedService;
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .IntentManagedFully()
            .AddUsing("System.Collections.Generic")
            .AddClass("PagedResult", @class =>
            {
                @class.AddGenericParameter("TData", out var tData);

                @class.AddConstructor(constructor =>
                {
                    constructor.AddStatement("Data = null!;");
                });

                @class.AddProperty("int", "TotalCount");
                @class.AddProperty("int", "PageCount");
                @class.AddProperty("int", "PageSize");
                @class.AddProperty("int", "PageNumber");
                @class.AddProperty($"List<{tData}>", "Data");
            });
    }

    public CSharpFile CSharpFile { get; }

    protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

    public override string TransformText() => CSharpFile.ToString();

    protected abstract IDesigner GetSourceDesigner(IMetadataManager metadataManager, string applicationId);

    public override bool CanRunTemplate()
    {
        return GetSourceDesigner(ExecutionContext.MetadataManager, ExecutionContext.GetApplicationConfig().Id)
            .GetServiceProxyModels()
            .SelectMany(s => _serviceProxyMappedService.GetMappedEndpoints(s))
            .Any(x => x.TypeReference?.Element?.Id == TypeDefinitionElementId);
    }

    private record MetadataModel(string Id) : IMetadataModel { }
}