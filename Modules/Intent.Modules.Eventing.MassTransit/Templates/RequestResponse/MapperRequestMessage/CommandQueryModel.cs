using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Eventing.MassTransit.Templates.RequestResponse.MapperRequestMessage;

public class CommandQueryModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder, IElementWrapper
{
    public CommandQueryModel(IElement element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (!IsCommandQueryElement(element))
        {
            throw new ArgumentException("Element is not a Command or a Query", nameof(element));
        }
        
        Id = element.Id;
        Name = element.Name;
        InternalElement = element;
        TypeReference = element.TypeReference;
        Folder = element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(element.ParentElement) : null!;
        Stereotypes = element.Stereotypes;
    }

    public static bool IsCommandQueryElement(IElement element)
    {
        return element.SpecializationType is "Command" or "Query";
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