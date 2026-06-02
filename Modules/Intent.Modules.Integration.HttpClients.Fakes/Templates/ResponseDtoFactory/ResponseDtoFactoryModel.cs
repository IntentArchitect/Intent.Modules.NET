using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Integration.HttpClients.Shared.Templates;

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.ResponseDtoFactory;

public class ResponseDtoFactoryModel : IMetadataModel, IHasName, IHasFolder, IElementWrapper
{
    public ResponseDtoFactoryModel(
        IServiceProxyModel serviceProxy,
        DTOModel dto,
        bool requiresCreateList,
        IReadOnlyCollection<string> cyclicTargetDtoIds,
        string? name = null,
        IReadOnlyDictionary<string, string>? factoryNamesByDtoId = null)
    {
        ServiceProxy = serviceProxy;
        Dto = dto;
        RequiresCreateList = requiresCreateList;
        CyclicTargetDtoIds = cyclicTargetDtoIds;
        Name = name ?? GetDefaultFactoryName(dto);
        FactoryNamesByDtoId = factoryNamesByDtoId ?? new Dictionary<string, string>
        {
            [dto.Id] = Name
        };
    }

    public string Id => $"{ServiceProxy.Id}:{Dto.Id}";
    public string Name { get; }
    public IServiceProxyModel ServiceProxy { get; }
    public DTOModel Dto { get; }
    public bool RequiresCreateList { get; }
    public IReadOnlyDictionary<string, string> FactoryNamesByDtoId { get; }

    /// <summary>
    /// Ids of DTOs referenced by <see cref="Dto"/> that can transitively reach back to it (including a
    /// direct self-reference). A field whose DTO type is in this set would otherwise produce infinite
    /// factory recursion, so the factory emits an empty/default placeholder for it instead.
    /// </summary>
    public IReadOnlyCollection<string> CyclicTargetDtoIds { get; }

    public FolderModel Folder => ServiceProxy.Folder;
    public IElement InternalElement => ServiceProxy.InternalElement;

    public static string GetDefaultFactoryName(DTOModel dto)
    {
        return $"{dto.Name.ToPascalCase()}Factory";
    }
}
