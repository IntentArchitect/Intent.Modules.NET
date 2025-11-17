using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.UnitTesting.Templates;

#nullable enable

/// <summary>
/// Wrapper class to provide IHasFolder functionality for IElement instances.
/// This mimics the behavior of the old strongly-typed model classes.
/// </summary>
internal class ElementWrapper : IHasFolder
{
    private readonly IElement _element;

    public ElementWrapper(IElement element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
        
        // Initialize folder if parent is a folder
        Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId 
            ? new FolderModel(_element.ParentElement) 
            : null;
    }

    public string Id => _element.Id;
    
    public string Name => _element.Name;
    
    public FolderModel? Folder { get; }

    public IElement? ParentElement => _element.ParentElement;
    
    public IEnumerable<string> GetParentFolderNames(params string[] folderNames)
    {
        var folders = new List<string>();
        var currentFolder = Folder;
        
        while (currentFolder != null)
        {
            folders.Insert(0, currentFolder.Name);
            currentFolder = currentFolder.Folder;
        }

        folders.AddRange(folderNames.Where(x => !string.IsNullOrWhiteSpace(x)));
        
        return folders;
    }
}
