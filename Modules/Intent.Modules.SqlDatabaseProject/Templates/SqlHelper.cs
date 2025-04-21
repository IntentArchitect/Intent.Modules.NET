using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.SqlDatabaseProject.Templates;

public static class SqlHelper
{
    public static bool TryGetSqlType(ITypeReference typeReference, out string? sqlType)
    {
        if (typeReference.HasIntType())
        {
            sqlType = "INT";
            return true;
        }

        if (typeReference.HasLongType())
        {
            sqlType = "BIGINT";
            return true;
        }

        if (typeReference.HasDateTimeType())
        {
            sqlType = "DATETIME";
            return true;
        }

        if (typeReference.HasGuidType())
        {
            sqlType = "UNIQUEIDENTIFIER";
            return true;
        }

        if (typeReference.HasBoolType())
        {
            sqlType = "BIT";
            return true;
        }
        
        if (typeReference.HasDecimalType())
        {
            sqlType = "DECIMAL";
            return true;
        }
        
        if (typeReference.HasDateType())
        {
            sqlType = "DATE";
            return true;
        }

        sqlType = null;
        return false;
    }
}