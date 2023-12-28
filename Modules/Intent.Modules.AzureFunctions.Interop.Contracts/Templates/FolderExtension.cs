using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.AzureFunctions.Interop.Contracts.Templates;

public static class FolderExtension
{
    public static FolderModel ToFolder(this ServiceModel serviceModel)
    {
        //return new FolderModel(new FolderDecoratedElement(serviceModel.InternalElement));
        return new FolderModel(serviceModel.InternalElement, serviceModel.InternalElement.SpecializationType);
    }
    
    // GCB - WTF is this trying to solve:
    //private class FolderDecoratedElement : IElement
    //{
    //    private readonly IElement _element;

    //    public FolderDecoratedElement(IElement element)
    //    {
    //        _element = element ?? throw new ArgumentNullException(nameof(element));
    //    }

    //    public bool IsChild => _element.IsChild;

    //    public int Order => _element.Order;

    //    public string ExternalReference => _element.ExternalReference;

    //    public string Value => _element.Value;

    //    public bool IsAbstract => _element.IsAbstract;

    //    public IEnumerable<IGenericType> GenericTypes => _element.GenericTypes;

    //    public ITypeReference TypeReference => _element.TypeReference;

    //    public IPackage Package => _element.Package;

    //    public string ParentId => _element.ParentId;

    //    public IElement ParentElement => _element.ParentElement;

    //    public IEnumerable<IElement> ChildElements => _element.ChildElements;

    //    public bool IsMapped => _element.IsMapped;

    //    public IElementMapping MappedElement => _element.MappedElement;

    //    public IElementApplication Application => _element.Application;

    //    public IEnumerable<IAssociationEnd> AssociatedElements => _element.AssociatedElements;

    //    public IEnumerable<IAssociation> OwnedAssociations => _element.OwnedAssociations;

    //    public IDiagram Diagram => _element.Diagram;

    //    public string SpecializationType => FolderModel.SpecializationType;
    //    public string SpecializationTypeId => FolderModel.SpecializationTypeId;
    //    public string Name => _element.Name;
    //    public string Comment => _element.Comment;

    //    public IDictionary<string, string> Metadata => _element.Metadata;
    //    public string Id => _element.Id;
    //    public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
    //}
}