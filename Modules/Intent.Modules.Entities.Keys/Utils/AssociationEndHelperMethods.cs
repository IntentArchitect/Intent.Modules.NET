using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Entities.Keys
{
    internal static class AssociationEndHelperMethods
    {
        public static bool RequiresForeignKey(this AssociationEndModel associationEnd)
        {
            return IsManyToVariantsOfOne(associationEnd) || IsSelfReferencingZeroToOne(associationEnd);
        }

        private static bool IsManyToVariantsOfOne(AssociationEndModel associationEnd)
        {
            return (associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne)
                   && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many;
        }

        private static bool IsSelfReferencingZeroToOne(AssociationEndModel associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.ZeroToOne && associationEnd.Association.TargetEnd.Class == associationEnd.Association.SourceEnd.Class;
        }
    }

    public static class ClassModelExtensions
    {
        public static IList<AttributeModel> GetExplicitPrimaryKey(this ClassModel @class)
        {
            return @class.Attributes.Where(x => x.HasPrimaryKey()).ToList();
        }

        public static string GetSurrogateKey(this ClassModel @class)
        {
            if (!HasSurrogateKey(@class))
            {
                throw new Exception($"{nameof(ClassModel)} [{@class}] does not have a surrogate key");
            }

            return @class.GetExplicitPrimaryKey().SingleOrDefault()?.Name ?? "Id";
        }

        public static string GetExplicitSurrogateKeyType(this ClassModel @class, ITypeResolver typeResolver)
        {
            if (!HasSurrogateKey(@class))
            {
                throw new Exception($"{nameof(ClassModel)} [{@class}] does not have a surrogate key");
            }

            return @class.GetExplicitPrimaryKey().Any() ?typeResolver.Get(@class.GetExplicitPrimaryKey().SingleOrDefault()?.Type.Element).Name : null;
        }

        public static bool HasSurrogateKey(this ClassModel @class)
        {
            return @class.Attributes.Count(x => x.HasPrimaryKey() && x.Name.EndsWith("id", StringComparison.InvariantCultureIgnoreCase)) <= 1; // 0 = implicit
        }
    }
}