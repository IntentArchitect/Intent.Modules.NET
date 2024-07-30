using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using static Intent.Modules.Constants.TemplateRoles.Domain;
using System.Linq.Expressions;
using Intent.Templates;

namespace Intent.Modules.Application.DomainInteractions;

public interface IDataAccessProvider
{
    CSharpStatement SaveChangesAsync();
    CSharpStatement AddEntity(string entityName);
    CSharpStatement Update(string entityName);
    CSharpStatement Remove(string entityName);
    CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNUllable, out IList<CSharpStatement> prerequisiteStatements);
    CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
}

public record PrimaryKeyFilterMapping(CSharpStatement ValueExpression, CSharpStatement Property, IElementToElementMappedEnd Mapping);

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

public static class CSharpClassExtensions
{
    public static CSharpClass GetRootEntity(this CSharpClass entity)
    {
        while (entity.BaseType != null && !entity.HasMetadata("primary-keys"))
        {
            entity = entity.BaseType;
        }

        return entity;
    }

    public static bool HasPrimaryKey(this CSharpClass rootEntity)
    {
        return rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks)
               && pks.Length > 0;
    }

    public static bool HasSinglePrimaryKey(this CSharpClass rootEntity)
    {
        return rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks)
               && pks.Length == 1;
    }

    public static CSharpProperty GetPropertyWithPrimaryKey(this CSharpClass rootEntity)
    {
        rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks);
        if (pks.Length == 0)
        {
            throw new InvalidOperationException($"Entity [{rootEntity.Name}] has no Primary Keys");
        }
        if (pks.Length > 1)
        {
            throw new InvalidOperationException($"Entity [{rootEntity.Name}] has more than one Primary Key");
        }
        return pks.First();
    }

    public static CSharpProperty[] GetPropertiesWithPrimaryKey(this CSharpClass rootEntity)
    {
        rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks);
        return pks;
    }
}