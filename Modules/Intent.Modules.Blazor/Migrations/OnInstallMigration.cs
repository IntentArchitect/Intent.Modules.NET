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
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.CrossPlatform.IO;
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
        private const string UIDesignerId = "f492faed-0665-4513-9853-5a230721786f";
        private const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";

        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string ServicePackageSpecializationId = "df45eaf6-9202-4c25-8dd5-677e9ba1e906";
        private const string UIPackageSpecializationId = "911c35b4-4ba3-404c-a0c6-e5258e53333a";
        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
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
            changes |= AddServicePackageReferenceToUIPackages(app);
            if (changes)
            {
                app.SaveAllChanges();
            }

        }

        private bool AddServicePackageReferenceToUIPackages(ApplicationPersistable app)
        {
            bool result = false;
            var designerServices = app.TryGetDesigner(ServicesDesignerId);
            if (designerServices == null)
                return result;
            var servicePackages = designerServices.GetPackages().Where(x => x.SpecializationTypeId == ServicePackageSpecializationId);
            if (!servicePackages.Any())
            {
                return result;
            }
            var designerUI = app.GetDesigner(UIDesignerId);
            var uipackages = designerUI.GetPackages().Where(x => x.SpecializationTypeId == UIPackageSpecializationId);

            foreach (var uiPackage in uipackages)
            {
                foreach (var servicePacakge in servicePackages)
                {
                    uiPackage.AddReference(PackageReferenceModel.Create(servicePacakge, true));
                    result = true;
                }
            }
            return result;
        }

        private static bool EnsureBlazorRoleInVSDesigner(ApplicationPersistable app)
        {
            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            StartupDetails? startupDetails = null;
            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(RoleSpecializationId);
                if (elements.Any(x => x.Name == "Blazor"))
                {
                    return false;
                }
                if (startupDetails == null)
                {
                    var startupRole = elements.FirstOrDefault(x => x.Name == "Startup");
                    if (startupRole != null)
                    {
                        startupDetails = new StartupDetails(package, startupRole);
                    }
                }
            }

            if (startupDetails != null)
            {
                var project = startupDetails.Role.Parent;
                project.AddElement(new ElementPersistable { SpecializationTypeId = RoleSpecializationId, SpecializationType = "Role", Name = "Blazor", Id = Guid.NewGuid().ToString() });
                return true;
            }
            else
            {
                throw new Exception("Could not find (Startup) or (Blazor) Role.");
            }
        }

        private record StartupDetails(PackageModelPersistable Pacakge, ElementPersistable Role);

        
    }
}
