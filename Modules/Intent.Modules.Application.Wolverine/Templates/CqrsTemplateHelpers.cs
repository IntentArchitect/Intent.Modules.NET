using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.Wolverine.Templates
{
    internal static class CqrsTemplateHelpers
    {
        public static bool ShouldSetDefaultValue(this DTOFieldModel property, int lastNonNullable)
        {
            return property.InternalElement.Order >= lastNonNullable && !string.IsNullOrEmpty(property.Value);
        }

        public static string GetTypeReferenceName(this DTOFieldModel field, bool setDefaultValue, IntentTemplateBase template)
        {
            var typeValue = template.GetTypeName(field.TypeReference);

            if (setDefaultValue && (field.TypeReference?.IsCollection ?? false) && (!field.TypeReference?.IsNullable ?? false))
            {
                typeValue = $"{template.GetTypeName(field.TypeReference)}?";
            }

            return typeValue;
        }
    }
}
