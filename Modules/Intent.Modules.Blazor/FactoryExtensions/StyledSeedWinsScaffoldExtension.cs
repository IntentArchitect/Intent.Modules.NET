using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Blazor.Templates.Templates.Client.StaticContentTemplateRegistrations;
using Intent.Modules.Blazor.Templates.Templates.Server.StaticContentTemplateRegistrations;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StyledSeedWinsScaffoldExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.StyledSeedWinsScaffoldExtension";

        public override int Order => 100;

        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changeManager;

        public StyledSeedWinsScaffoldExtension(ISoftwareFactoryEventDispatcher sfEventDispatcher, IChanges changeManager)
        {
            _sfEventDispatcher = sfEventDispatcher;
            _changeManager = changeManager;
        }

        // The styled non-Mud page seeds (server + WASM, sample + no-sample). Mud apps don't register
        // these (they ship a generated Home + framework styling), so iterating these ids is self-gating.
        private static readonly string[] SeedRegistrationIds =
        {
            SamplePagesStaticContentTemplateRegistration.TemplateId,
            NoSamplePagesStaticContentTemplateRegistration.TemplateId,
            WasmSamplePagesStaticContentTemplateRegistration.TemplateId,
            WasmNoSampleStaticContentTemplateRegistration.TemplateId,
        };

        protected override void OnAfterTemplateExecution(IApplication application)
        {
            // Component-library (e.g. MudBlazor) apps style differently and ship a generated Home;
            // their page seeds are not registered. Nothing to do.
            if (TemplateHelper.ComponentLibraryInstalled(application))
            {
                return;
            }

            var landed = 0;
            var seen = 0;
            foreach (var registrationId in SeedRegistrationIds)
            {
                foreach (var seed in application.FindTemplateInstances<ITemplate>(registrationId))
                {
                    var filePath = seed.GetMetadata().GetFilePath();
                    var fileName = Path.GetFileName(filePath);
                    if (!IsStyleableSeedFile(fileName))
                    {
                        continue;
                    }

                    seen++;
                    var styledContent = seed.RunTemplate();
                    var onDisk = File.Exists(filePath) ? File.ReadAllText(filePath) : null;
                    var change = _changeManager.FindChange(filePath);

                    if (onDisk != null && !IsStockScaffold(fileName, onDisk))
                    {
                        // Customised (or already styled) — never clobber.
                        change?.ChangeContent(onDisk, onDisk);
                        continue;
                    }

                    // Absent or recognisably-stock → land the styled content.
                    if (change != null)
                    {
                        change.ChangeContent(styledContent, styledContent);
                    }
                    else
                    {
                        _sfEventDispatcher.Publish(new SoftwareFactoryEvent(
                            SoftwareFactoryEvents.OverwriteFileCommand,
                            new Dictionary<string, string>
                            {
                                { "FullFileName", filePath },
                                { "Content", styledContent },
                            }));
                    }

                    landed++;
                }
            }

            if (seen == 0)
            {
                Logging.Log.Warning(
                    $"{nameof(StyledSeedWinsScaffoldExtension)}: found no non-Mud page-seed instances " +
                    "(Home.razor/MainLayout.razor/app.css). Styling may not have been applied.");
            }
            else
            {
                Logging.Log.Info($"{nameof(StyledSeedWinsScaffoldExtension)}: re-styled {landed} of {seen} stock seed file(s).");
            }
        }

        private static bool IsStyleableSeedFile(string fileName) =>
            fileName.Equals("Home.razor", StringComparison.OrdinalIgnoreCase) ||
            fileName.Equals("MainLayout.razor", StringComparison.OrdinalIgnoreCase) ||
            fileName.Equals("app.css", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// True only when the on-disk file is still the recognisable stock "dotnet new" Blazor
        /// scaffold, so customised files are preserved. Uses structural markers (not byte-exact) to
        /// tolerate SDK-version differences in the scaffold.
        /// </summary>
        private static bool IsStockScaffold(string fileName, string content)
        {
            if (fileName.Equals("Home.razor", StringComparison.OrdinalIgnoreCase))
            {
                // Stock renders the framework "Hello, world!"; the styled seed renders the hero/bento.
                return content.Contains("Hello, world!");
            }

            if (fileName.Equals("MainLayout.razor", StringComparison.OrdinalIgnoreCase))
            {
                // Stock top-row links to the ASP.NET Core docs; the styled layout replaces it with a
                // ThemeToggle + AppUserMenu.
                return content.Contains("learn.microsoft.com/aspnet/core") && !content.Contains("ThemeToggle");
            }

            if (fileName.Equals("app.css", StringComparison.OrdinalIgnoreCase))
            {
                // The styled design system is built entirely on CSS custom properties; the stock
                // scaffold uses none.
                return !content.Contains("var(--");
            }

            return false;
        }
    }
}
