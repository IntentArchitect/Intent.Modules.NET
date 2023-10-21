using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;

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
}