using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.Entities.Keys
{
    internal static class SurrogateTypeHelper
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
    }
}
