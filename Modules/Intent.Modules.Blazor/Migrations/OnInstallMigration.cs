using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IMetadataInstaller _metadataInstaller;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IMetadataInstaller metadataInstaller)
        {
            _configurationProvider = configurationProvider;
            _metadataInstaller = metadataInstaller;
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            if (app == null)
                return;

            bool changes = false;
            changes |= EnsureBlazorRoleInVSDesigner(app);
            changes |= MigrationHelper.InitializeIncludeSamplesSetting(app, "true");
            if (changes)
            {
                app.SaveAllChanges();
            }

            /*
            var visualStudioMetadataInstallationFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../content", "visual-studio.metadata.config");
            if (File.Exists(visualStudioMetadataInstallationFile))
            {
                _metadataInstaller.InstallFromFile(app.AbsolutePath, visualStudioMetadataInstallationFile,
                    fieldDictionary: new Dictionary<string, string>
                    {
                        ["application.name"] = _configurationProvider.GetApplicationConfig().Name,
                        ["solution.name"] = _configurationProvider.GetSolutionConfig().SolutionName
                    }, s => { return Task.CompletedTask; });
            }
            else
            {
                throw new Exception("File not found: " + visualStudioMetadataInstallationFile);
            }*/
        }

        private static bool EnsureBlazorRoleInVSDesigner(ApplicationPersistable app)
        {
            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            ElementPersistable? startupRole = null;
            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(RoleSpecializationId);
                if (elements.Any(x => x.Name == "Blazor"))
                {
                    return false;
                }
                if (startupRole == null)
                {
                    startupRole = elements.FirstOrDefault(x => x.Name == "Startup");
                }
            }

            if (startupRole != null)
            {
                var package = packages.First(p => p.Id == startupRole.PackageId);
                var project = startupRole.Parent;
                project.AddElement(new ElementPersistable { SpecializationTypeId = RoleSpecializationId, SpecializationType = "Role", Name = "Blazor" });
                return true;
            }
            else
            {
                throw new Exception("Could not find (Startup) or (Blazor) Role.");
            }
        }

        public void OnUninstall()
        {
            // TODO: Should uninstall client?
            //var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            //var designer = app.GetDesigner(VisualStudioDesignerId);
            //var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            //foreach (var package in packages)
            //{
            //    var elements = package.GetElementsOfType(RoleSpecializationId);
            //    if (elements.Any(x => x.Name == "Blazor"))
            //    {
            //        return;
            //    }
            //}
        }
    }
}
