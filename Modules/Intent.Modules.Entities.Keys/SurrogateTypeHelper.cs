using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.Entities.Keys
{
    internal static class SurrogateTypeHelper
    {
        public static string GetSurrogateKeyType(this ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
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

        public static string GetSurrogateKeyType(this ICSharpTemplate template)
        {
            return template.UseType(GetSurrogateKeyType(template.ExecutionContext));
        }
    }
}
