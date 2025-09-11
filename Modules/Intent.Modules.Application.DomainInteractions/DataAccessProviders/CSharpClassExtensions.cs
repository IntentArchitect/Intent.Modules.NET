using System;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

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

        return pks.Length switch
        {
            0 => throw new InvalidOperationException($"Entity [{rootEntity.Name}] has no Primary Keys"),
            > 1 => throw new InvalidOperationException($"Entity [{rootEntity.Name}] has more than one Primary Key"),
            _ => pks[0]
        };
    }

    public static CSharpProperty[] GetPropertiesWithPrimaryKey(this CSharpClass rootEntity)
    {
        rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks);
        return pks;
    }
}