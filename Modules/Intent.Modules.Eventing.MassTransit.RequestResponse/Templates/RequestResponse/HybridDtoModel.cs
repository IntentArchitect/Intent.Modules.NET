using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse;

public class HybridDtoModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder, IElementWrapper
{
    public HybridDtoModel(IElement element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (!IsHybridDtoModel(element))
        {
            throw new ArgumentException("Element is not a Command / Query / DTO", nameof(element));
        }
        
        Id = element.Id;
        Name = element.Name;
        InternalElement = element;
        TypeReference = element.TypeReference;
        Folder = element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(element.ParentElement) : null!;
        Stereotypes = element.Stereotypes;
    }

    public static bool IsHybridDtoModel(IElement element)
    {
        return element.SpecializationType is "Command" or "Query" or "DTO";
    }

    public string Id { get; }
    public string Name { get; }
    public IElement InternalElement { get; }
    public ITypeReference TypeReference { get; }
    public FolderModel Folder { get; }
    public IEnumerable<IStereotype> Stereotypes { get; }
    
    public IList<DTOFieldModel> Properties => InternalElement.ChildElements
        .GetElementsOfType(DTOFieldModel.SpecializationTypeId)
        .Select(x => new DTOFieldModel(x))
        .ToList();
}