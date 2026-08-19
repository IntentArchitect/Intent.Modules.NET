using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.DefaultContent;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentStyle;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HomePageDefaultContentFactoryExtension : FactoryExtensionBase
    {
        // The modelled Home page is identified by the file name RazorComponentTemplate computes for it,
        // rather than by a path check, so it stays correct regardless of which folder it is modelled in.
        private const string HomeOutputFileName = "Home";

        public override string Id => "Intent.Blazor.HomePageDefaultContentFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // A component-library module (e.g. MudBlazor) owns the home page's look and seeds its own
            // default content — the plain-HTML seed below would fight with it.
            if (TemplateHelper.ComponentLibraryInstalled(application))
            {
                return;
            }

            var homeTemplate = ComponentTemplateIds.AllClientRazorTemplateIds
                .SelectMany(application.FindTemplateInstances<ComponentRazorTemplateBase>)
                .FirstOrDefault(x => string.Equals(
                    ComponentRazorTemplateBase.GetOutputFileName(x.Model),
                    HomeOutputFileName,
                    StringComparison.OrdinalIgnoreCase));

            if (homeTemplate is null)
            {
                return;
            }

            // Once-off by construction: RazorComponentTemplate.TransformText() only consults
            // DefaultContentOverride when the .razor file does not yet exist on disk.
            homeTemplate.DefaultContentOverride = HomePageContent.BuildRazorContent(homeTemplate);

            var styleTemplate = application
                .FindTemplateInstances<RazorComponentStyleTemplate>(RazorComponentStyleTemplate.TemplateId)
                .FirstOrDefault(x => x.Model.InternalElement.Id == homeTemplate.Model.InternalElement.Id);

            if (styleTemplate is null || File.Exists(styleTemplate.GetMetadata().GetFilePath()))
            {
                // RazorComponentStyleTemplate pins no OverwriteBehaviour, so seeding an existing file
                // would overwrite whatever the developer has since written into it.
                return;
            }

            styleTemplate.StyleContentOverride = HomePageContent.BuildStyleContent(homeTemplate);
        }
    }
}
