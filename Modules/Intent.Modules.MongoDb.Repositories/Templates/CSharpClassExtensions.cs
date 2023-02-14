using System;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.MongoDb.Repositories.Templates;

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
}