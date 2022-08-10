using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.Dtos.AutoMapper.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.Dtos.AutoMapper;

public static class SurrogateKeyHelper
{
    public static string GetSurrogateKeyType(this IntentTemplateBase template)
    {
        var settingType = template.ExecutionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
        switch (settingType)
        {
            case "guid":
                return "System.Guid";
            case "int":
                return "int";
            case "long":
                return "long";
            default:
                return settingType;
        }
    }
    
    public static bool IsSurrogateKeyType(this IntentTemplateBase template, string typeName)
    {
        var typeNameMapped = MapTypeNameToLanguageType(typeName);
        return template.GetSurrogateKeyType().Equals(typeNameMapped);
    }

    public static IReadOnlyCollection<AttributeModel> GetExplicitPrimaryKey(this ClassModel classModel)
    {
        return classModel.Attributes
            .Where(p => p.HasStereotype("Primary Key"))
            .ToList();
    }

    private static string MapTypeNameToLanguageType(string typeName)
    {
        var typeNameMapped = typeName?.ToLower() switch
        { 
            "guid" => "System.Guid", 
            "int" => "int", 
            "long" => "long", 
            _ => typeName
        };
        return typeNameMapped;
    }
}