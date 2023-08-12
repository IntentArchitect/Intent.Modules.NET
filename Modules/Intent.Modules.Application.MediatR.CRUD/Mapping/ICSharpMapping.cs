using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public interface ICSharpMapping
{
    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappingConnection Mapping { get; set; }
    IEnumerable<CSharpStatement> GetMappingStatement(
        IDictionary<ICanBeReferencedType, string> fromReplacements,
        IDictionary<ICanBeReferencedType, string> toReplacements);

    void AddFromReplacement(ICanBeReferencedType type, string replacement);
    void AddToReplacement(ICanBeReferencedType type, string replacement);
}