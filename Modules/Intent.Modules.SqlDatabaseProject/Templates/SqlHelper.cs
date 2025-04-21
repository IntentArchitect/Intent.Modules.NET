using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
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
    
    public static string? FindSchema(this IHasStereotypes targetElement)
    {
        IHasStereotypes currentElement = targetElement;

        if (currentElement.HasStereotype("Table") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("Table", "Schema")?.Trim()))
        {
            return currentElement.GetStereotypeProperty<string>("Table", "Schema")?.Trim();
        }

        if (currentElement.HasStereotype("View") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("View", "Schema")?.Trim()))
        {
            return currentElement.GetStereotypeProperty<string>("View", "Schema")?.Trim();
        }

        while (currentElement != null)
        {
            if (currentElement.HasStereotype("Schema"))
            {
                return currentElement.GetStereotypeProperty<string>("Schema", "Name")?.Trim();
            }

            if (currentElement is not IElement element)
            {
                break;
            }

            currentElement = element.ParentElement ?? (IHasStereotypes)element.Package;
        }
        return null;
    }
}