using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Blazor.Templates
{
    internal class TemplateHelper
    {
        // This is the base component library marker; component-library modules install it.
        private const string ComponentLibraryModuleId = "Intent.Modelers.UI.Core";

        internal static bool ComponentLibraryInstalled(IApplication application)
        {
            return application.InstalledModules.Any(x => x.ModuleId == ComponentLibraryModuleId);
        }

        /// <summary>
        /// Whether <c>wwwroot/app.css</c> is actually generated for this application.
        /// </summary>
        /// <remarks>
        /// The single source of truth for the app.css condition. It is shared by the two content
        /// groups that ship the file (<c>NoSamplePages</c> for Interactive Server,
        /// <c>WasmNoSamplePages</c> for Interactive WebAssembly) and by AppRazorTemplate, which emits
        /// its &lt;link&gt;. Previously the link was unconditional while the file was gated, so every
        /// application with a component library installed - and every Interactive Auto application,
        /// which neither content group covers - 404'd on app.css on every page load.
        ///
        /// Keep the two in step through this method. If a content group's own condition changes,
        /// change it here.
        /// </remarks>
        internal static bool ShipsAppCss(IApplication application)
        {
            return !ComponentLibraryInstalled(application) && ShipsAppCssForRenderMode(application.GetSettings());
        }

        /// <inheritdoc cref="ShipsAppCss(IApplication)"/>
        internal static bool ShipsAppCss(ISoftwareFactoryExecutionContext executionContext)
        {
            return executionContext.InstalledModules.All(x => x.ModuleId != ComponentLibraryModuleId) &&
                   ShipsAppCssForRenderMode(executionContext.Settings);
        }

        private static bool ShipsAppCssForRenderMode(IApplicationSettingsProvider settings)
        {
            var renderMode = settings.GetBlazor().RenderMode();

            // Interactive Auto is deliberately absent: no content group ships app.css for it.
            return renderMode.IsInteractiveServer() || renderMode.IsInteractiveWebAssembly();
        }
    }
}
