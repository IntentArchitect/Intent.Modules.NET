using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent
{
    /// <summary>
    /// Resolves the modelled Home page's Razor template instance across every Client razor
    /// variant (plain Component or Page) by its computed output file name, now that Pages
    /// generate through <c>PageTemplate</c> rather than always through <c>RazorComponentTemplate</c>.
    /// </summary>
    public static class HomeTemplateLookup
    {
        public static IAuthPageRazorTemplate? FindByOutputFileName(IApplication application, string outputFileName)
        {
            return ComponentTemplateIds.AllClientRazorTemplateIds
                .SelectMany(application.FindTemplateInstances<IAuthPageRazorTemplate>)
                .FirstOrDefault(x => string.Equals(
                    ComponentRazorTemplateBase.GetOutputFileName(x.Model),
                    outputFileName,
                    StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finds the generated Razor file (across every Client razor variant) whose output path
        /// ends with <paramref name="fileNameSuffix"/> (e.g. "Home.razor").
        /// </summary>
        public static IRazorFileTemplate? FindRazorFileByPathSuffix(IApplication application, string fileNameSuffix)
        {
            return ComponentTemplateIds.AllClientRazorTemplateIds
                .SelectMany(application.FindTemplateInstances<IRazorFileTemplate>)
                .FirstOrDefault(t => t.GetMetadata().GetFilePath().EndsWith(fileNameSuffix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
