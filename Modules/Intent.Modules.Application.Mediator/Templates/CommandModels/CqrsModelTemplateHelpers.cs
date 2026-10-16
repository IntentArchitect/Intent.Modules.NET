using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Security.Shared;

namespace Intent.Modules.Application.Mediator.Templates
{
    internal static class CqrsModelTemplateHelpers
    {
        public const string IdentityAuthorizeAttributeTemplateId = "Intent.Application.Identity.AuthorizeAttribute";

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

        public static void AddAuthorization(ICSharpTemplate template, CSharpClass @class, IElement element, string conceptName)
        {
            foreach (var securityModel in SecurityModelHelpers.GetSecurityModels(element))
            {
                if (!template.TryGetTypeName(IdentityAuthorizeAttributeTemplateId, out var authorizeAttributeName))
                {
                    throw new ElementException(element,
                        $"The {conceptName} '{element.Name}' is secured, but authorization requires the Intent.Application.Identity module, which is not installed. " +
                        "Install Intent.Application.Identity or remove the Secured stereotype from this element.");
                }

                @class.AddAttribute(
                    authorizeAttributeName.RemoveSuffix("Attribute"),
                    attribute =>
                    {
                        if (securityModel.Roles.Count > 0)
                        {
                            attribute.AddArgument($"Roles = {SecurityHelper.RolesToPermissionConstants(securityModel.Roles, template)}");
                        }

                        if (securityModel.Policies.Count > 0)
                        {
                            attribute.AddArgument($"Policy = {SecurityHelper.PoliciesToPermissionConstants(securityModel.Policies, template)}");
                        }
                    });
            }
        }

        public static DefaultValueAttributeKind GetDefaultValueAttributeKind(this DTOFieldModel property)
        {
            if (property.TypeReference?.IsCollection == true) return DefaultValueAttributeKind.None;
            var element = property.TypeReference?.Element;
            if (element == null) return DefaultValueAttributeKind.None;

            if (element.IsEnumModel() ||
                element.IsStringType() ||
                element.IsBoolType() ||
                element.IsCharType() ||
                element.IsIntType() ||
                element.IsLongType() ||
                element.IsFloatType() ||
                element.IsDoubleType() ||
                element.IsDecimalType())
                return DefaultValueAttributeKind.Simple;

            if (element.IsDateType() ||
                element.IsDateTimeType() ||
                element.IsDateTimeOffsetType() ||
                element.IsGuidType())
                return DefaultValueAttributeKind.TypeAndString;

            return DefaultValueAttributeKind.None;
        }
    }

    internal enum DefaultValueAttributeKind
    {
        None,
        Simple,
        TypeAndString
    }
}
