using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Entities;
public static class TypeReferenceExtensions
{
    public static bool IsPrimitiveType(this ITypeReference typeReference) => typeReference.HasIntType() || typeReference.HasStringType() || typeReference.HasGuidType() || typeReference.HasDateType() ||
            typeReference.HasDateTimeType() || typeReference.HasBoolType() || typeReference.HasDoubleType() || typeReference.HasDecimalType() ||
            typeReference.HasFloatType() || typeReference.HasShortType() || typeReference.HasBinaryType() || typeReference.HasByteType() ||
            typeReference.HasLongType() || typeReference.HasCharType() || typeReference.HasDateTimeOffsetType();
}
