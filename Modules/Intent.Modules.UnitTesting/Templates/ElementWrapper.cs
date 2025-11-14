using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.UnitTesting.Templates;

/// <summary>
/// Wrapper class to provide IHasFolder functionality for IElement instances.
/// This mimics the behavior of the old strongly-typed model classes.
/// </summary>
internal class ElementWrapper : IHasFolder
{
    private readonly IElement _element;
    private readonly FolderModel _folder;

    public ElementWrapper(IElement element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
        
        // Initialize folder if parent is a folder
        _folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId 
            ? new FolderModel(_element.ParentElement) 
            : null;
    }

    public string Id => _element.Id;
    
    public string Name => _element.Name;
    
    public FolderModel Folder => _folder;
    
    public IEnumerable<string> GetParentFolderNames()
    {
        var folders = new List<string>();
        var currentFolder = _folder;
        
        while (currentFolder != null)
        {
            folders.Insert(0, currentFolder.Name);
            currentFolder = currentFolder.Folder;
        }
        
        return folders;
    }
}
