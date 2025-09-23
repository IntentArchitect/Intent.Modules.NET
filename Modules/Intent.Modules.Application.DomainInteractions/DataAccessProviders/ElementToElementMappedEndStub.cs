using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

internal record ElementToElementMappedEndStub : IElementToElementMappedEnd
{
    private readonly ICanBeReferencedType _sourceElement;
    private readonly ICanBeReferencedType _targetElement;
    public ElementToElementMappedEndStub(ICanBeReferencedType sourceElement, ICanBeReferencedType targetElement)
    {
        _sourceElement = sourceElement;
        _targetElement = targetElement;
    }

    public string MappingType => throw new NotImplementedException();

    public string MappingTypeId => throw new NotImplementedException();

    public string MappingExpression => throw new NotImplementedException();

    public IList<IElementMappingPathTarget> TargetPath => throw new NotImplementedException();

    public ICanBeReferencedType TargetElement => _targetElement;

    public IEnumerable<IElementToElementMappedEndSource> Sources => throw new NotImplementedException();

    public IList<IElementMappingPathTarget> SourcePath => throw new NotImplementedException();

    public ICanBeReferencedType SourceElement => _sourceElement;

    public IElementToElementMappedEndSource GetSource(string identifier)
    {
        throw new NotImplementedException();
    }
}