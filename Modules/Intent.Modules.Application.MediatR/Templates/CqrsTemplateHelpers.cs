using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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
}