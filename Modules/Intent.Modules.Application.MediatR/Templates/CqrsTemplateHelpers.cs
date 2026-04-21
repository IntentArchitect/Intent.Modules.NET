using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Security.Shared;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

namespace Intent.Modules.Application.MediatR.Templates;

internal static class CqrsTemplateHelpers
{
    public static string GetCommandFolderPath(this CSharpTemplateBase<CommandModel> template)
    {
        return template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
            ? template.GetFolderPath()
            : template.GetFolderPath(additionalFolders: template.Model.GetConceptName());
    }

    public static string GetCommandNamespace(this CSharpTemplateBase<CommandModel> template)
    {
        return template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
            ? template.GetNamespace()
            : template.GetNamespace(additionalFolders: template.Model.GetConceptName());
    }

    public static string GetQueryFolderPath(this CSharpTemplateBase<QueryModel> template)
    {
        return template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
            ? template.GetFolderPath()
            : template.GetFolderPath(additionalFolders: template.Model.GetConceptName());
    }

    public static string GetQueryNamespace(this CSharpTemplateBase<QueryModel> template)
    {
        return template.ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile()
            ? template.GetNamespace()
            : template.GetNamespace(additionalFolders: template.Model.GetConceptName());
    }

    public static void AddAuthorization(ICSharpTemplate template, CSharpClass @class, IElement element)
    {
        foreach (var securityModel in SecurityModelHelpers.GetSecurityModels(element))
        {
            @class.AddAttribute(
                template.TryGetTypeName("Application.Identity.AuthorizeAttribute", out var @out)
                    ? @out.RemoveSuffix("Attribute")
                    : "Authorize",
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

    public static bool ShouldSetDefaultValue(this DTOFieldModel property, int lastNonNullable)
    {
        // should the default value be set, based on the position of it as a argument
        return property.InternalElement.Order >= lastNonNullable && !string.IsNullOrEmpty(property.Value);
    }

    public static string GetTypeReferenceName(this DTOFieldModel field, bool setDefaultValue, IntentTemplateBase template)
    {
        // set the type
        var typeValue = template.GetTypeName(field.TypeReference);

        // if we are setting the default value, and the type is a collection, we need to make it nullable in order to set the default value to null
        if (setDefaultValue && (field.TypeReference?.IsCollection ?? false) && (!field.TypeReference?.IsNullable ?? false))
        {
            typeValue = $"{template.GetTypeName(field.TypeReference)}?";
        }

        return typeValue;
    }

    /// <summary>
    /// Determines how a property's default value should be represented in a <c>[DefaultValue(...)]</c> attribute.
    /// <list type="bullet">
    ///   <item><see cref="DefaultValueAttributeKind.Simple"/> — a direct literal argument, e.g. <c>[DefaultValue(42)]</c></item>
    ///   <item><see cref="DefaultValueAttributeKind.TypeAndString"/> — the type+string overload, e.g. <c>[DefaultValue(typeof(DateTime), "2024-01-01")]</c></item>
    ///   <item><see cref="DefaultValueAttributeKind.None"/> — the attribute cannot represent this type and should be omitted</item>
    /// </list>
    /// </summary>
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